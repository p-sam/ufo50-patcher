#load "_Utils.csx"

using System;

Version GetUFO50Version(UndertaleData utdata) {
    var constants = GetConstants(utdata, new string[] {"@@VersionMajor", "@@VersionMinor", "@@VersionRevision"});
    return Version.Parse(String.Join('.', constants));
}

UndertaleString GetUFO50DisplayVersionString(UndertaleData utdata) {
    var scrInitCode = utdata.Code.ByName("gml_GlobalScript_scrInit");
    if(scrInitCode == null) {
        throw new ScriptException("scrInitCode == null");
    }

    for(int i = 1; i < scrInitCode.Instructions.Count; i++) {
        var popInst = scrInitCode.Instructions[i];
        if (popInst.Kind != UndertaleInstruction.Opcode.Pop) {
            continue;
        }

        if (popInst.Type1 != UndertaleInstruction.DataType.Variable) {
            continue;
        }

        if (popInst.Type2 != UndertaleInstruction.DataType.String) {
            continue;
        }

        if (popInst.Destination.ToString() != "betaVersion") {
            continue;
        }

        var pushInst = scrInitCode.Instructions[i-1];
        if (pushInst.Kind != UndertaleInstruction.Opcode.Push) {
            continue;
        }

        if (!(pushInst.Value is UndertaleResourceRef)) {
            continue;
        }

        var resourceRef = (UndertaleResourceRef)pushInst.Value;
        if (!(resourceRef.Resource is UndertaleString)) {
            continue;
        }

        return (UndertaleString)resourceRef.Resource;
    }

    throw new ScriptException("failed to find display version string");
}
