﻿namespace Northwind.Features.IndexingRelationships.Graph
{
    public class Number
    {
        public string Id { get; set; }

        public string[] FollowedBy { get; set; }
    }
}