﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PathFind
{
    public static class PathFind
    {
        public static Path<Node> FindPath<Node>(
            Node start,
            Node destination,
            Func<Node, Node, double> distance,
            Func<Node, double> estimate)
            where Node : IHasNeighbours<Node>
        {
            var closed = new HashSet<Node>();
            var queue = new PriorityQueue<double, Path<Node>>();
            queue.Enqueue(0, new Path<Node>(start));
			
			
            while (!queue.IsEmpty)
            {
                var path = queue.Dequeue();
				
				
				
                if (closed.Contains(path.LastStep))
				{
					//Debug.Log("FindPath-continue");
                    continue;
				}
				
                if (path.LastStep.Equals(destination))
				{
					//Debug.Log("FindPath-Return path:");
                    return path;
				}
				
				//Debug.Log("FindPath-Add");
                closed.Add(path.LastStep);

                foreach (Node n in path.LastStep.Neighbours)
                {
                    double d = distance(path.LastStep, n);
                    var newPath = path.AddStep(n, d);
                    queue.Enqueue(newPath.TotalCost + estimate(n), newPath);
					
					//Debug.Log("		FindPath-Add-Loop ");
					
                }
            }
			
			//Debug.Log("FindPath-return null ----");
            return null;
        }
    }
}