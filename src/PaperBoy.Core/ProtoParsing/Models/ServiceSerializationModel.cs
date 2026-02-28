using System.Collections.Generic;

namespace PaperBoy.Core.ProtoParsing.Models;

internal record ServiceSerializationModel(
    Dictionary<string, Dictionary<string, Dictionary<string, object>>> ProtoService);