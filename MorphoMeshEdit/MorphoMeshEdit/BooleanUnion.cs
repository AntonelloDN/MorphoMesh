using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using MeshMorphoLib;
using g3;

namespace MorphoMesh
{
    public class BooleanUnion : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public BooleanUnion()
          : base("BooleanUnion", "BooleanUnion",
              "Use this component to run Boolean Union or Boolean Difference from multiple geometries.",
              "MorphoMesh", "1 | Operation")
        {
            this.Message = "VER 0.0.01\nDEC_10_2018";
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("meshes", "meshes", "Connect multiple Brep or Mesh.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("num", "num", "Number of cells to use for calculation, larger numbers mean better shape approximation.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("runIt", "runIt", "Set it to true to run the calculation.", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("showIt", "showIt", "Set it to true to show Grasshopper result.", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("writeObj", "writeObj", "Write the result in a obj file, this could be faster than Grasshopper calc.", GH_ParamAccess.item, false);
            pManager.AddTextParameter("path", "path", "Write the absolute path where to write the file.", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("result", "result", "Grasshopper mesh.", GH_ParamAccess.item);
            pManager.AddGenericParameter("filePath", "filePath", "Path where the file obj is.", GH_ParamAccess.item);
            pManager.HideParameter(0);
            pManager.HideParameter(1);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // INPUT
            // declaration
            List<Rhino.Geometry.Mesh> meshes = new List<Rhino.Geometry.Mesh>();
            int num = 0;
            bool runIt = false;
            bool showIt = false;
            bool writeObj = false;
            string path = null;

            DA.GetDataList<Rhino.Geometry.Mesh>(0, meshes);
            DA.GetData(1, ref num);
            DA.GetData(2, ref runIt);
            DA.GetData(3, ref showIt);
            DA.GetData(4, ref writeObj);
            DA.GetData(5, ref path);

            // run

            if (runIt)
            {
                Rhino.Geometry.Mesh mesh = MeshMorphoLib.MeshIntegration.UnionMesh(meshes);

                List<g3.Vector3f> vertici;
                int[] triangoli;
                var vettori = MeshMorphoLib.MeshIntegration.DecomposeRhinoMesh(mesh, out triangoli, out vertici);

                var g3mesh = MeshMorphoLib.MeshClassIO.CreaMesh(vertici, triangoli, vettori);

                var newMesh = MeshMorphoLib.MeshClassFnc.BooleanUnion(g3mesh, num);

                if (showIt)
                {
                    Rhino.Geometry.Mesh resultMesh = MeshMorphoLib.MeshIntegration.ConvertToRhinoMesh(newMesh);
                    DA.SetData(0, resultMesh);
                }
                if (writeObj)
                {
                    try
                    {
                        string fullFolder = System.IO.Path.Combine(path, "MorphoModel.obj");
                        MeshClassIO.WriteMesh(newMesh, fullFolder);
                        DA.SetData(1, fullFolder);
                    }
                    catch
                    {
                        this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Please provide a valid path.");
                    }
                }
            }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return MorphoMeshEdit.Properties.Resources.BooleanUnion;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("ba36fdbf-a09f-43c2-9242-467aa9687c9e"); }
        }
    }
}