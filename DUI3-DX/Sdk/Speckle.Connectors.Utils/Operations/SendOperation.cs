﻿using Speckle.Connectors.Utils.Builders;
using Speckle.Core.Models;

namespace Speckle.Connectors.Utils.Operations;

public sealed class SendOperation<T>
{
  private readonly IRootObjectBuilder<T> _rootObjectBuilder;
  private readonly IRootObjectSender _baseObjectSender;
  private readonly ISyncToThread _syncToThread;

  public SendOperation(
    IRootObjectBuilder<T> rootObjectBuilder,
    IRootObjectSender baseObjectSender,
    ISyncToThread syncToThread
  )
  {
    _rootObjectBuilder = rootObjectBuilder;
    _baseObjectSender = baseObjectSender;
    _syncToThread = syncToThread;
  }

  public async Task<(string rootObjId, Dictionary<string, ObjectReference> convertedReferences)> Execute(
    IReadOnlyList<T> objects,
    SendInfo sendInfo,
    Action<string, double?>? onOperationProgressed = null,
    CancellationToken ct = default
  )
  {
    Base commitObject = await _syncToThread
      .RunOnThread(() => _rootObjectBuilder.Build(objects, sendInfo, onOperationProgressed, ct))
      .ConfigureAwait(false);

    // base object handler is separated so we can do some testing on non-production databases
    // exact interface may want to be tweaked when we implement this
    return await _baseObjectSender.Send(commitObject, sendInfo, onOperationProgressed, ct).ConfigureAwait(false);
  }
}