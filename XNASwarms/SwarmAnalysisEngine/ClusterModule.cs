﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SwarmEngine;
using Microsoft.Xna.Framework;

namespace SwarmAnalysisEngine
{
    public class ClusterModule : AnalysisModule
    {
        int ClusterItemThreshhold = 8;
        int ClusterBackCount = 30;
        List<AnalysisMessage> ReadOut = new List<AnalysisMessage>(); 
        public List<Cluster> Clusters;
        private Analysis analysis;
        double leftMostX, rightMostX, topMostY, bottomMostY;
        List<Individual> lastfew;
        FilterResult filterresult;
        BoundingSphere sphere;
        BoundingBox rect;

        public ClusterModule()
            : base("Cluster Module", 4)
        {
            Clusters = new List<Cluster>();
            List<AnalysisMessage> ReadOut = new List<AnalysisMessage>();
            analysis = new Analysis();
        }
        
        protected override Analysis Analyze(List<Individual> indvds)
        {
            return DoAnalysis(indvds);
        }

        private void ResetColor(Individual indvd)
        {
            indvd.setDisplayColor(Color.MidnightBlue);
        }


        private Analysis DoAnalysis(List<Individual> indvds)
        {
            Clusters.Clear();

            Clusters.Add(new Cluster() { indvds[0] });

            for (int i = 0; i < indvds.Count; i++)
            {
                ResetColor(indvds[i]);
                if (!InExistingCluster(indvds[i]))
                {
                    Clusters.Add(new Cluster() { indvds[i] });
                }
            }

            RemoveSmallClusters();
            SetClusterColor();

            analysis.Messages = GenerateMessage();
            analysis.FilterResult = GenerateFilterResult();

            return analysis;
        }

        private void RemoveSmallClusters()
        {
            Clusters.RemoveAll(s => s.Count() <= ClusterItemThreshhold);
        }

        private bool InExistingCluster(Individual individual)
        {
            List<int> clustersIds = new List<int>();
            lastfew = new List<Individual>();

            for (int c = 0; c < Clusters.Count; c++)
            {
                lastfew = Clusters[c].Skip(Math.Max(0, Clusters[c].Count() - ClusterBackCount)).Take(ClusterBackCount).ToList();

                for (int i = 0; i < lastfew.Count(); i++)
                {
                    double newdis = (individual.X - lastfew[i].X) * (individual.X - lastfew[i].X) + (individual.Y - lastfew[i].Y) * (individual.Y - lastfew[i].Y);

                    if (newdis < lastfew[i].getGenome().getNeighborhoodRadius() * lastfew[i].getGenome().getNeighborhoodRadius())
                    {
                        clustersIds.Add(c);
                        break;
                    }
                }
            }

            if (clustersIds.Count > 0)
            {
                if (clustersIds.Count > 1)
                {
                    //Merge the clusters                    
                    Clusters[clustersIds[0]].AddRange(Clusters[clustersIds[1]]);
                    Clusters.RemoveAt(clustersIds[1]);
                    Clusters[clustersIds[0]].Add(individual);
                    return true;
                }
                Clusters[clustersIds[0]].Add(individual);
                return true;
            }
            return false;
        }

        private void SetClusterColor()
        {
            for (int clusterid = 0; clusterid < Clusters.Count; clusterid++)
            {
                foreach (Individual indvd in Clusters[clusterid])
                {
                    if (clusterid == 0)
                    {
                        indvd.setDisplayColor(Color.Red);
                    }
                    else if (clusterid == 1)
                    {
                        indvd.setDisplayColor(Color.Chartreuse);
                    }
                    else if (clusterid == 2)
                    {
                        indvd.setDisplayColor(Color.Yellow);
                    }
                    else if (clusterid == 3)
                    {
                        indvd.setDisplayColor(Color.Orange);
                    }
                    else if (clusterid == 4)
                    {
                        indvd.setDisplayColor(Color.Lerp(Color.LightPink, Color.LightBlue, clusterid * .15f));
                    }
                    else
                    {
                        indvd.setDisplayColor(Color.Lerp(Color.LawnGreen, Color.Cyan, (float)((clusterid - 5) * .25)));
                    }
                }
            }
        }

        private List<AnalysisMessage> GenerateMessage()
        {
            ReadOut.Clear();

            for (int i = 0; i < Clusters.Count; i++)
            {
                string clusterVisualCount = "";
                int reducedClusterCount = Clusters[i].Count() / 5;
                for (int c = 0; c < reducedClusterCount; c++)
                {
                    if (reducedClusterCount - c > 10)
                    {
                        clusterVisualCount += "||";
                    }
                    else
                    {
                        clusterVisualCount += " |";
                    }
                    
                }
                ReadOut.Add(new AnalysisMessage() { Type = this.ModuleName, Message = "COUNT : " + clusterVisualCount + "  " + reducedClusterCount * 5  });
            }
            ReadOut.Add(new AnalysisMessage() { Type = "              ", Message = "                                                  " });
            return ReadOut;
        }

        private FilterResult GenerateFilterResult()
        {
            filterresult = new FilterResult() { Type = FilterType.ClusterCenter, ClusterCenters = new List<Vector2>() };

            foreach (Cluster cluster in Clusters)
            {
                leftMostX = cluster.OrderBy(point => point.X).First().X;
                rightMostX = cluster.OrderByDescending(point => point.X).First().X;
                topMostY = cluster.OrderBy(point => point.Y).First().Y;
                bottomMostY = cluster.OrderByDescending(point => point.Y).First().Y;

                float verticalCenter = (float)(topMostY + bottomMostY) * .5f;
                float horizontalCenter = (float)(leftMostX + rightMostX) * .5f;

                AssignClusterCenterPoint(new Vector2(horizontalCenter, verticalCenter));

                //2|1
                //3|4

                //1
                AssignQuadCenterPoint(cluster.Where(i => i.X > horizontalCenter && i.Y < verticalCenter));
                //2
                AssignQuadCenterPoint(cluster.Where(i => i.X < horizontalCenter && i.Y < verticalCenter));
                //3
                AssignQuadCenterPoint(cluster.Where(i => i.X < horizontalCenter && i.Y > verticalCenter));
                //4
                AssignQuadCenterPoint(cluster.Where(i => i.X > horizontalCenter && i.Y > verticalCenter));              
            }
            return filterresult;
        }


        private void AssignQuadCenterPoint(IEnumerable<Individual> quadItems)
        {
            if (quadItems.Count() > 0)
            {
                leftMostX = quadItems.OrderBy(point => point.X).First().X;
                rightMostX = quadItems.OrderByDescending(point => point.X).First().X;
                topMostY = quadItems.OrderBy(point => point.Y).First().Y;
                bottomMostY = quadItems.OrderByDescending(point => point.Y).First().Y;

                Vector3[] positions;

                positions = new Vector3[3] {
                        new Vector3((float)(leftMostX + rightMostX) * .5f, (float)(topMostY + bottomMostY) * .5f,0),
                        new Vector3((float)leftMostX, (float)(bottomMostY + topMostY)*.5f, 0),
                        new Vector3((float)(leftMostX + rightMostX) * .5f, (float)topMostY, 0)
                    };

                filterresult.ClusterCenters.Add(GetSphereDifference(positions));
            }
        }

        private Vector2 GetSphereDifference(Vector3[] positions)
        {
            BoundingSphere sphere = BoundingSphere.CreateFromPoints(positions);
            return new Vector2(sphere.Center.X,sphere.Center.Y);
            
        }

        private void AssignClusterCenterPoint(Vector2 clusterCenter)
        {
            filterresult.ClusterCenters.Add(clusterCenter);
        }
    }
}
