using System.Collections.Generic;
using RhythmEngine.Data;
using TMPro;
using UnityEngine;

public class LeaderboardController : MonoBehaviour
{
    public TextMeshProUGUI leaderboardText;
    public int topN = 10;

    public void SetSongLeaderboard(SongItem song)
    {
        string displayText = "";
        string currentSongCode = song.songCode;
        // displayText += $"--{song.songName}--\n\n";

        KeyValuePair<string, int>[] topScores = Leaderboard.GetTopScores(currentSongCode, topN).ToArray();

        if(topScores.Length == 0)
        {
            Leaderboard.InitializeLeaderboard(currentSongCode);
            topScores = Leaderboard.GetTopScores(currentSongCode, topN).ToArray();
        }

        foreach (var entry in topScores)
        {
            displayText += $"{entry.Key} - {entry.Value} pts\n";
        }
        leaderboardText.text = displayText;
    }
}

public static class Leaderboard
{
    //Formato de tabla, lista de keyvalue, donde el key es el nombre del jugador y el value es su puntuación
    //Al cargar se lee y ordena la lista por puntuación, de mayor a menor, y se muestra en la tabla
    //Guardado en .json, una lista de nombres y puntunaciones, sin orden, solo insertada al final del archivo
    //Estilo
    //Alex;1000
    //Maria;800
    //Juan;2000

    public const string extension = ".leaderboard";
    public static string FilePath => Application.persistentDataPath;

    public static string GetFilePathForSong(string songCode)
    {
        return System.IO.Path.Combine(FilePath, $"{songCode}{extension}");
    }


    public static void AddScore(string songCode, string playerName, int score)
    {
        try
        {
            string line = $"{playerName};{score}";
            string filePath = GetFilePathForSong(songCode);
            System.IO.File.AppendAllText(filePath, line + System.Environment.NewLine);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error adding score to leaderboard: {ex.Message}");
        }
    }

    public static List<KeyValuePair<string, int>> GetTopScores(string songCode, int topN)
    {
        var scores = new List<KeyValuePair<string, int>>();
        
        try
        {
            string filePath = GetFilePathForSong(songCode);
            if (!System.IO.File.Exists(filePath))
                return scores;

            string[] lines = System.IO.File.ReadAllLines(filePath);
            
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(';');
                if (parts.Length == 2 && int.TryParse(parts[1], out int score))
                {
                    scores.Add(new KeyValuePair<string, int>(parts[0], score));
                }
            }
            
            // Ordenar de mayor a menor puntuación
            scores.Sort((a, b) => b.Value.CompareTo(a.Value));
            
            // Devolver solo los top N
            return scores.GetRange(0, Mathf.Min(topN, scores.Count));
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error reading leaderboard: {ex.Message}");
            return scores;
        }
    }


    public static void InitializeLeaderboard(string songCode)
    {
        try
        {
            string filePath = GetFilePathForSong(songCode);
            if (!System.IO.File.Exists(filePath))
            {
                System.IO.File.Create(filePath).Close();
                // Se inicializa vacío, las puntuaciones se añadirán durante el juego
                AddScore(songCode, "Alex", 1000); // Entrada inicial para evitar archivos vacíos
                AddScore(songCode, "Maria", 856);
                AddScore(songCode, "Juan", 823);
                AddScore(songCode, "Jose", 500);
                AddScore(songCode, "Ana", 400);
                AddScore(songCode, "Pedro", 1);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error initializing leaderboard for song {songCode}: {ex.Message}");
        }
    }
    
}