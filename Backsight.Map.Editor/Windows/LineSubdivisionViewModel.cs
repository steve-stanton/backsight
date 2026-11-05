using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Backsight.Map.Editor.Tools;
using Backsight.Model;
using Backsight.Model.Observations;
using Backsight.Model.Operations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Backsight.Map.Editor.Windows;

internal partial class LineSubdivisionViewModel : ToolViewModel<LineSubdivisionTool>
{
    /// <summary>
    /// The line to be subdivided.
    /// </summary>
    internal LineFeature Line { get; }

    /// <summary>
    /// The data entry units
    /// </summary>
    private DistanceUnit EntryUnits { get; }
    
    internal string GroundLength { get; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OkCommand))]
    private string? _distanceText;

    /// <summary>
    /// The number of distances entered so far.
    /// </summary>
    private int _distanceCount;
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OkCommand))]
    private string? _validationError;

    [ObservableProperty]
    private bool _isFromStart = true;
        
    [ObservableProperty]
    private string? _enteredLength;
    
    [ObservableProperty]
    private string? _remainingLength;

    internal LineSubdivisionViewModel(
        LineSubdivisionTool tool,
        LineFeature line,
        LineSubdivisionOperation? recall) : base(tool)
    {
        Line = line;
        EntryUnits = DistanceUnit.GetUnit(tool.Store.Settings.EntryUnit);
        GroundLength = EntryUnits.Format(Line.GroundLength.Meters, true, -1);

        if (recall is not null)
            DistanceText = GetEntryString(recall, EntryUnits);
    }

    /// <summary>
    /// Helper for <c>GetEntryString</c>.
    /// </summary>
    /// <param name="text">The text string to be counted.</param>
    private class StringCount(string text)
    {
        internal string Text { get; } = text;
        internal int Count { get; set; } = 1;
    }
    
    private static string GetEntryString(LineSubdivisionOperation op, DistanceUnit entryUnits)
    {
        // Collate each distance along with a repeat count 

        var lastStringCount = new StringCount(String.Empty);
        var items = new List<StringCount>();

        foreach (var distance in op.Face.Sections.Select(x => x.ObservedLength))
        {
            var appendAbbrev = !ReferenceEquals(distance.EntryUnit, entryUnits);
            var distanceText = distance.Format(appendAbbrev);

            if (lastStringCount.Text == distanceText)
            {
                lastStringCount.Count++;
            }
            else
            {
                lastStringCount = new StringCount(distanceText);
                items.Add(lastStringCount);
            }
        }

        // Generate the entry string with suitable repeat counts

        var sb = new StringBuilder();
        foreach (var item in items)
        {
            if (sb.Length > 0)
                sb.Append(' ');
            
            sb.Append(item.Text);
            if (item.Count > 1)
                sb.Append($"*{item.Count}");
        }

        return sb.ToString();
    }
    
    partial void OnDistanceTextChanged(string? value)
    {
        ValidationError = null;
        var distances = GetDistances();

        if (ValidationError is null)
        {
            // We will need at least two distances to subdivide a line
            _distanceCount = distances.Length;
            
            var enteredLength = distances.Sum(d => d.Meters);
            EnteredLength = EntryUnits.Format(enteredLength, true, -1);

            var remaining = Line.GroundLength.Meters - enteredLength;
            RemainingLength = EntryUnits.Format(remaining, true, -1);
            
            Tool.RefreshMapDisplay();
        }
    }
    
    partial void OnIsFromStartChanged(bool value)
    {
        Tool.RefreshMapDisplay();
    }
    
    protected override bool CanExecuteOk()
    {
        return DistanceText is not null && ValidationError is null && _distanceCount > 1;
    }

    /// <summary>
    /// Parses the distances list box.
    /// </summary>
    /// <returns>The parsed distances (an empty array if a parsing error occurs).</returns>
    internal Distance[] GetDistances()
    {
        try
        {
            string entryString = GetEntryString();
            return LineSubdivisionFace.GetDistances(entryString, EntryUnits, !IsFromStart);
        }
        catch (Exception e)
        {
            ValidationError = e.Message;
        }
        return [];
    }

    /// <summary>
    /// Gets a string holding the currently entered distances.
    /// </summary>
    /// <returns>The entered distances (each separated by a space character).</returns>
    string GetEntryString()
    {
        var text = DistanceText?.ReplaceLineEndings(" ");
        if (String.IsNullOrWhiteSpace(text))
            return String.Empty;

        var result = new StringBuilder(1000);

        foreach (string t in text.Split(" "))
        {
            string s = t.Trim();

            // Skip empty lines
            if (s.Length == 0)
                continue;

            // The user may have entered the *, about to append a repeat count. In that
            // case, ignore the trailing *.
            if (s.EndsWith("*"))
                s = s.Substring(0, t.Length - 1);

            if (result.Length > 0)
                result.Append(" ");

            result.Append(s);
        }

        return result.ToString();
    }
}