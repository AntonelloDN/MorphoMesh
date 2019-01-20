using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using g3;


namespace MeshMorphoLib
{
    public class MeshClassFnc
    {
        public static DMesh3 BooleanUnion(DMesh3 mesh, int num_cells)
        {
            // credits to geometry3sharp team
            double cell_size = mesh.CachedBounds.MaxDim / num_cells;

            MeshSignedDistanceGrid sdf = new MeshSignedDistanceGrid(mesh, cell_size);
            sdf.Compute();

            var iso = new DenseGridTrilinearImplicit(sdf.Grid, sdf.GridOrigin, sdf.CellSize);

            DMesh3 outputMesh  = GenerateMeshF(iso, num_cells);

            return outputMesh;
        }


        public static DMesh3 MineCraft(DMesh3 mesh, int num_cells, out double scalefactor)
        {
            DMeshAABBTree3 spatial = new DMeshAABBTree3(mesh, autoBuild: true);


            AxisAlignedBox3d bounds = mesh.CachedBounds;
            double cellsize = bounds.MaxDim / num_cells;

            scalefactor = cellsize;

            ShiftGridIndexer3 indexer = new ShiftGridIndexer3(bounds.Min, cellsize);

            Bitmap3 bmp = new Bitmap3(new Vector3i(num_cells, num_cells, num_cells));
            foreach (Vector3i idx in bmp.Indices())
            {
                g3.Vector3d v = indexer.FromGrid(idx);
                if (spatial.IsInside(v))
                {
                    bmp.Set(idx, true);
                }
                else
                {
                    bmp.Set(idx, false);
                }
            }

            VoxelSurfaceGenerator voxGen = new VoxelSurfaceGenerator();
            voxGen.Voxels = bmp;
            voxGen.Generate();

            DMesh3 voxMesh = voxGen.Meshes[0];

            return voxMesh;
        }


        public static DMesh3 ReduceMesh(DMesh3 mesh, int count)
        {
            Reducer outputMesh = new Reducer(mesh);
            outputMesh.ReduceToTriangleCount(count);

            return new g3.DMesh3(outputMesh.Mesh, true);
        }


        public static DMesh3 Remesh(DMesh3 mesh, double edgeLenght, int iteration)
        {
            Remesher r = new Remesher(mesh);
            r.PreventNormalFlips = true;
            r.SetTargetEdgeLength(edgeLenght);
            for (int k = 0; k < iteration; ++k)
                r.BasicRemeshPass();

            return new g3.DMesh3(r.Mesh, true);
        }


        public static BoundedImplicitFunction3d MeshToImplicitF(DMesh3 mesh, int num_cells, double offset)
        {

            double meshCellsize = mesh.CachedBounds.MaxDim / num_cells;
            MeshSignedDistanceGrid levelSet = new MeshSignedDistanceGrid(mesh, meshCellsize);
            levelSet.ExactBandWidth = (int)(offset / meshCellsize) + 1;
            levelSet.Compute();
            return new DenseGridTrilinearImplicit(levelSet.Grid, levelSet.GridOrigin, levelSet.CellSize);

        }


        public static DMesh3 GenerateMeshF(BoundedImplicitFunction3d mesh, int num_cells)
        {
            MarchingCubes c = new MarchingCubes();
            c.Implicit = mesh;
            c.RootMode = MarchingCubes.RootfindingModes.LerpSteps;      // cube-edge convergence method
            c.RootModeSteps = 5;                                        // number of iterations
            c.Bounds = mesh.Bounds();
            c.CubeSize = c.Bounds.MaxDim / num_cells;
            c.Bounds.Expand(3 * c.CubeSize);                            // leave a buffer of cells
            c.Generate();
            MeshNormals.QuickCompute(c.Mesh);                           // generate normals

            DMesh3 outputMesh = c.Mesh;
            return outputMesh;
        }


        public static ImplicitUnion3d ImplicitUnion(BoundedImplicitFunction3d meshA, BoundedImplicitFunction3d meshB)
        {

            var union = new ImplicitUnion3d() { A = meshA, B = meshB };

            return union;
        }


        public static ImplicitDifference3d ImplicitDifference(BoundedImplicitFunction3d meshA, BoundedImplicitFunction3d meshB)
        {

            var diff = new ImplicitDifference3d() { A = meshA, B = meshB };

            return diff;
        }


        public static ImplicitIntersection3d ImplicitIntersection(BoundedImplicitFunction3d meshA, BoundedImplicitFunction3d meshB)
        {

            var intersection = new ImplicitIntersection3d() { A = meshA, B = meshB };

            return intersection;
        }


        public static ImplicitBlend3d ImplicitBlend(BoundedImplicitFunction3d meshA, BoundedImplicitFunction3d meshB, double blend)
        {

            var blendMesh = new ImplicitBlend3d() { A = meshA, B = meshB, Blend = blend };

            return blendMesh;
        }


        public static ImplicitOffset3d ImplicitOffset(BoundedImplicitFunction3d mesh,  double offset)
        {

            var offsetMesh = new ImplicitOffset3d() { A = mesh, Offset = offset };

            return offsetMesh;
        }

        
        public static ImplicitSmoothDifference3d ImplicitSmoothDifference(BoundedImplicitFunction3d meshA, BoundedImplicitFunction3d meshB)
        {
            
            var smoothMesh = new ImplicitSmoothDifference3d() { A = meshA, B = meshB };

            return smoothMesh;
        }

    }

    public class MeshClassIO
    {
        public static DMesh3 CreaMesh(List<Vector3f> vertices, int[] triangles, List<Vector3f> normals)
        {
            return DMesh3Builder.Build(vertices, triangles, normals);
        }

        public static void WriteMesh(DMesh3 inputMesh, string path)
        {
            StandardMeshWriter.WriteMesh(path, inputMesh, WriteOptions.Defaults);
        }
    }
}
