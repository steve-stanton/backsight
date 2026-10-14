using Backsight.Database;
using Backsight.Environment;

namespace Backsight.Map.Editor.Windows;

/// <summary>
/// Dialog to allow specification of default entity type for a map.
/// </summary>
public partial class GetEntityWindow : DialogWindow<GetEntityViewModel>
{
    /// <summary>
    /// Design-time constructor.
    /// </summary>
    public GetEntityWindow() : base(null!)
    {
        InitializeComponent();
    }

    internal GetEntityWindow(IEnvironmentRepository envRepo, ILayer layer, int defaultEntityId, SpatialType t)
        : base(new GetEntityViewModel(envRepo, layer, defaultEntityId, t))
    {
        InitializeComponent();
    }
}