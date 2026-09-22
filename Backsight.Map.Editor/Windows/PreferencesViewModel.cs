using System;
using Backsight.Map.Editor.Models;
using Backsight.Model;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Backsight.Map.Editor.Windows;

public partial class PreferencesViewModel : DialogViewModel
{
    private const string SymbologyScaleKey = nameof(SymbologyScale);
    
    private readonly IMapEditorModel _model;
    private readonly MapSettings _settings;

    // Points tab
    [ObservableProperty] private double _pointScale;
    [ObservableProperty] private double _pointHeight;
    [ObservableProperty] private bool _showIntersections;
    
    // Labels tab
    [ObservableProperty] private double _labelScale;
    [ObservableProperty] private uint _nominalMapScale;
    [ObservableProperty] private string _defaultFont;
    
    // Units tab
    [ObservableProperty] private DistanceUnitType _entryUnit;
    [ObservableProperty] private DistanceUnitType _displayUnit;
    
    // Line Annotation tab
    [ObservableProperty] private double _annotationScale;
    [ObservableProperty] private double _annotationHeight;
    [ObservableProperty] private bool _showObservedAngles;
    [ObservableProperty] private LineAnnotationOptions _annotationLengthDisplay;
    
    // Symbology tab
    [ObservableProperty] private int _symbologyScale;
    
    internal PreferencesViewModel(IMapEditorModel model)
    {
        _model = model ?? throw new ArgumentNullException(nameof(model));
        _settings = _model.Store?.Settings ?? throw new InvalidOperationException("No map settings");

        // Points tab
        _pointScale = _settings.PointScale;
        _pointHeight = _settings.PointHeight;
        _showIntersections = _settings.IntersectionsDrawn;
        
        // Labels tab
        _labelScale = _settings.LabelScale;
        _nominalMapScale = _settings.NominalMapScale;
        
        // Units tab
        _entryUnit = _settings.EntryUnit;
        _displayUnit = _settings.DisplayUnit;

        // Line Annotation tab
        _annotationScale = _settings.LineAnnotation.ShowScale;
        _annotationHeight = _settings.LineAnnotation.Height;
        _showObservedAngles = _settings.LineAnnotation.ShowObservedAngles;

        if (_settings.LineAnnotation.ShowAdjustedLengths)
            _annotationLengthDisplay = LineAnnotationOptions.ShowAdjustedLengths;
        else if (_settings.LineAnnotation.ShowObservedLengths)
            _annotationLengthDisplay = LineAnnotationOptions.ShowObservedLengths;
        else
            _annotationLengthDisplay = LineAnnotationOptions.None;

        // Symbology tab
        _symbologyScale = GlobalUserSetting.ReadInt(SymbologyScaleKey, 5000);
    }

    internal void SaveChanges()
    {
        // Points tab
        _settings.PointScale = PointScale;
        _settings.PointHeight = PointHeight;
        _settings.IntersectionsDrawn = ShowIntersections;

        // Labels tab
        _settings.LabelScale = LabelScale;
        _settings.NominalMapScale = NominalMapScale;
        
        // Units tab
        _settings.EntryUnit = EntryUnit;
        _settings.DisplayUnit = DisplayUnit;
        
        // Line Annotation tab
        _settings.LineAnnotation.ShowScale = AnnotationScale;
        _settings.LineAnnotation.Height = AnnotationHeight;
        _settings.LineAnnotation.ShowObservedAngles = ShowObservedAngles;
        
        _settings.LineAnnotation.ShowAdjustedLengths = false;
        _settings.LineAnnotation.ShowObservedLengths = false;
        
        if (AnnotationLengthDisplay == LineAnnotationOptions.ShowAdjustedLengths)
            _settings.LineAnnotation.ShowAdjustedLengths = true;
        else if (AnnotationLengthDisplay == LineAnnotationOptions.ShowObservedLengths)
            _settings.LineAnnotation.ShowObservedLengths = true;
        
        // Symbology tab (applies to all maps)
        GlobalUserSetting.WriteInt(SymbologyScaleKey, SymbologyScale);

        // And save
        _model.MapRepository.SaveMapSettings(_model.MapName, _settings);
    }
}