using System;

// Place this file in any folder that is or is a descendant of a folder named "Scripts"
namespace RPG2DAttributes
{
    // Restrict to methods only
    [AttributeUsage(AttributeTargets.Property)]
    public class ShowTogglePropertyAttribute : Attribute
    {
    }

    // Restrict to methods only
    [AttributeUsage(AttributeTargets.Property)]
    public class ShowNumberPropertyAttribute : Attribute
    {
    }

    // Restrict to methods only
    [AttributeUsage(AttributeTargets.Property)]
    public class ShowStringPropertyAttribute : Attribute
    {
    }
}