using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Rhino;
using g3;


namespace MeshMorphoLib
{
    public class MeshIntegration
    {

        public static Rhino.Geometry.Mesh UnionMesh(List<Rhino.Geometry.Mesh> meshes)
        {
            Rhino.Geometry.Mesh outMesh = new Rhino.Geometry.Mesh();

            foreach (Rhino.Geometry.Mesh m in meshes)
                outMesh.Append(m);

            return outMesh;
        }

        public static List<g3.Vector3f> DecomposeRhinoMesh(Rhino.Geometry.Mesh mesh, out int[] triangles, out List<g3.Vector3f> vertices)
        {
            // triangles
            triangles = mesh.Faces.ToIntArray(true);

            // vertices and vectors
            List<g3.Vector3f> vectors = new List<g3.Vector3f>();
            vertices = new List<g3.Vector3f>();

            var verticiMesh = mesh.Vertices;
            var vettoriMesh = mesh.Normals;

            for (int i = 0; i < verticiMesh.Count; i++)
            {
                vertices.Add(new g3.Vector3f((float)verticiMesh[i].X, (float)verticiMesh[i].Y, (float)verticiMesh[i].Z));
                vectors.Add(new g3.Vector3f((float)vettoriMesh[i].X, (float)vettoriMesh[i].Y, (float)vettoriMesh[i].Z));
            }

            return vectors;
        }

        public static Rhino.Geometry.Mesh ConvertToRhinoMesh(DMesh3 mesh)
        {
            Rhino.Geometry.Mesh meshGH = new Rhino.Geometry.Mesh();

            var verticiG3 = mesh.Vertices();
            var triangoliG3 = mesh.Triangles();

            var facceGh = triangoliG3.ToArray().Select(v => new Rhino.Geometry.MeshFace(v.a, v.b, v.c));
            var verticiGh = verticiG3.ToArray().Select(v => new Rhino.Geometry.Point3d(v.x, v.y, v.z));

            meshGH.Faces.AddFaces(facceGh);
            meshGH.Vertices.AddVertices(verticiGh);
            meshGH.Normals.ComputeNormals();
            meshGH.Compact();

            return meshGH;
        }
    }
}
