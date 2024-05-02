using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Speckle.Connectors.DUI.Bindings;
using Speckle.Connectors.DUI.Bridge;
using Speckle.Connectors.DUI.Models;
using Speckle.Connectors.Revit.HostApp;
using Speckle.Connectors.Revit.Plugin;
using Speckle.Converters.RevitShared.Helpers;

namespace Speckle.Connectors.Revit.Bindings;

// POC: we need a base a RevitBaseBinding
internal class SelectionBinding : RevitBaseBinding, ISelectionBinding
{
  private readonly IRevitIdleManager _revitIdleManager;

  public SelectionBinding(
    RevitContext revitContext,
    DocumentModelStore store,
    IRevitIdleManager idleManager,
    IBridge bridge
  )
    : base("selectionBinding", store, bridge, revitContext)
  {
    _revitIdleManager = idleManager;

    // POC: we can inject the solution here
    // TODO: Need to figure it out equivalent of SelectionChanged for Revit2020
    _revitContext.UIApplication.SelectionChanged += (_, _) => _revitIdleManager.SubscribeToIdle(OnSelectionChanged);

    _revitContext.UIApplication.ViewActivated += (_, _) =>
    {
      Parent.Send(SelectionBindingEvents.SET_SELECTION, new SelectionInfo());
    };
  }

  private void OnSelectionChanged()
  {
    Parent.Send(SelectionBindingEvents.SET_SELECTION, GetSelection());
  }

  public SelectionInfo GetSelection()
  {
    // POC: this was also being called on shutdown
    // probably the bridge needs to be able to know if the plugin has been terminated
    // also on termination the OnSelectionChanged event needs unwinding
    List<Element> els = _revitContext.UIApplication.ActiveUIDocument.Selection
      .GetElementIds()
      .Select(id => _revitContext.UIApplication.ActiveUIDocument.Document.GetElement(id))
      .ToList();
    List<string> cats = els.Select(el => el.Category?.Name ?? el.Name).Distinct().ToList();
    List<string> ids = els.Select(el => el.UniqueId.ToString()).ToList();
    return new SelectionInfo()
    {
      SelectedObjectIds = ids,
      Summary = $"{els.Count} objects ({string.Join(", ", cats)})"
    };
  }
}
