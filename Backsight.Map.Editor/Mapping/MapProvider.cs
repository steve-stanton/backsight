using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backsight.Map.Editor.Models;
using Backsight.Map.Editor.ViewModels;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Providers;

namespace Backsight.Map.Editor.Mapping;

public class MapProvider : IProvider
{
    private readonly IMapEditorViewModel _viewModel;
    private readonly IMapEditorModel  _model;
    
    public MapProvider(IMapEditorViewModel viewModel, IMapEditorModel model)
    {
        _viewModel = viewModel;
        _model = model;
    }
    
    MRect? IProvider.GetExtent()
    {
        return _model.Extent?.ToMRect();
    }

    Task<IEnumerable<IFeature>> IProvider.GetFeaturesAsync(FetchInfo fetchInfo)
    {
        var result = new List<IFeature>();
        var mapScale = GetMapScale(fetchInfo);
        
        return Task.FromResult<IEnumerable<IFeature>>(result);
    }

    string? IProvider.CRS
    {
        get => "EPSG:26914"; // UTM zone 14 (assume we're in Manitoba)
        set => throw new NotImplementedException();
    }

    /// <summary>
    /// Determines the map scale for a fetch.
    /// </summary>
    /// <param name="fetchInfo"></param>
    /// <param name="dpi">The number of pixels per inch for the display.</param>
    /// <returns>The corresponding map scale denominator.</returns>
    private static double GetMapScale(FetchInfo fetchInfo, double dpi = 96.0)
    {
        double pixelsToMeters = 0.0254 / dpi;
        var width = fetchInfo.Section.ScreenWidth * pixelsToMeters;
        return fetchInfo.Extent.Width / width;
    }
}