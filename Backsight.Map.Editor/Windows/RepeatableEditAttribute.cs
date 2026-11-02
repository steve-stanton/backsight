using System;
using Backsight.Model;

namespace Backsight.Map.Editor.Windows;

/// <summary>
/// Tags a method used to launch an editing operation.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public class RepeatableEditAttribute(EditingActionId editId) : Attribute
{
    public EditingActionId EditId { get; } = editId;
}