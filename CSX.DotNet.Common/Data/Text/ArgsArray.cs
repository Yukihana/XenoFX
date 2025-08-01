using CSX.DotNet.Common.Data.Collections;

namespace CSX.DotNet.Common.Data.Text;

public class ArgsArray : ArrayWrapper<string>
{
    public ArgsArray(string[] args) : base(args)
    {
    }
}