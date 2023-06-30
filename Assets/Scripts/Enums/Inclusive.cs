public enum Inclusive
{
    /// <summary>
    /// Значение больше min, но меньше max 
    /// </summary>
    None,
    /// <summary>
    /// Значение больше или равно min, но меньше max 
    /// </summary>
    Minimum,
    /// <summary>
    /// Значение больше min, но меньше или равно max 
    /// </summary>
    Maximum,
    /// <summary>
    /// Значение больше или равно min, но меньше или равно max 
    /// </summary>
    All
}
