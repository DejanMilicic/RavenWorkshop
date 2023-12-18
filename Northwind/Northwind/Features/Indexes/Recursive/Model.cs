using System.Collections.Generic;

namespace Northwind.Features.Indexes.Recursive;

public class Part
{
    public string Id { get; set; }

    public string[] SubParts { get; set; }
}

