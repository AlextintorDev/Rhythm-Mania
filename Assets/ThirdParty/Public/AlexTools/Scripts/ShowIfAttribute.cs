using UnityEngine;

/// <summary>
/// Atttribute for show a field if other field is true or false.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
public sealed class ShowIfAttribute : PropertyAttribute
{
    public string ConditionalSourceField;
    public bool expectedValue;
    public bool HideInInspector;

    /// <summary>
    /// Create the attribute for show a field x if field y is true or false.
    /// </summary>
    /// <param name="ConditionalSourceField">name of field y type boolean </param>
    /// <param name="expectedValue"> what value should have the field y for show the field x</param>
    /// <param name="HideInInspector"> if should hide in the inspector or only disable</param>
    public ShowIfAttribute(string ConditionalSourceField, bool expectedValue, bool HideInInspector = false)
    {
        this.ConditionalSourceField = ConditionalSourceField;
        this.expectedValue = expectedValue;
        this.HideInInspector = HideInInspector;
    }
}