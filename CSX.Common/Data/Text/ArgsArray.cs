using CSX.Common.Data.Collections;

namespace CSX.Common.Data.Text;

public class ArgsArray : ArrayWrapper<string>
{
    public ArgsArray(string[] args) : base(args)
    {
    }
}