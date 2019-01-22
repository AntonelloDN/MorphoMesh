using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using MeshMorphoLib;
using g3;

namespace MorphoMeshEdit
{
    public class ImplicitUnion : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public ImplicitUnion()
          : base("ImplicitUnion", "ImplicitUnion",
              "Use this component to run Boolean Union between two meshes.",
              "MorphoMesh", "1 | Operation")
        {
            this.Message = "VER 0.0.01\nGEN_20_2018";
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("meshA", "meshA", "Connect a Brep or Mesh.", GH_ParamAccess.item);
            pManager.AddMeshParameter("meshB", "meshB", "Connect a Brep or Mesh.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("num", "num", "Number of cells to use for calculation, larger numbers mean better shape approximation.", GH_ParamAccess.item);
            pManager.AddNumberParameter("offset", "offset", "Connect a double if you want to smooth geometries.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("runIt", "runIt", "Set it to true to run the calculation.", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("showIt", "showIt", "Set it to true to show Grasshopper result.", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("writeObj", "writeObj", "Write the result in a obj file, this could be faster than Grasshopper calc.", GH_ParamAccess.item, false);
            pManager.AddTextParameter("path", "path", "Write the absolute path where to write the file.", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;
            pManager[7].Optional = true;
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
            Rhino.Geometry.Mesh meshA = null;
            Rhino.Geometry.Mesh meshB = null;
            int num = 0;
            double offset = 0.00;
            bool runIt = false;
            bool showIt = false;
            bool writeObj = false;
            string path = null;

            DA.GetData(0, ref meshA);
            DA.GetData(1, ref meshB);
            DA.GetData(2, ref num);
            DA.GetData(3, ref offset);
            DA.GetData(4, ref runIt);
            DA.GetData(5, ref showIt);
            DA.GetData(6, ref writeObj);
            DA.GetData(7, ref path);

            // run

            if (runIt)
            {
                DMesh3 g3MeshA = ConvertDMesh(meshA);
                DMesh3 g3MeshB = ConvertDMesh(meshB);

                BoundedImplicitFunction3d implicitA = MeshMorphoLib.MeshClassFnc.MeshToImplicitF(g3MeshA, num, offset);
                BoundedImplicitFunction3d implicitB = MeshMorphoLib.MeshClassFnc.MeshToImplicitF(g3MeshB, num, offset);


                var implicitUnionDone = MeshMorphoLib.MeshClassFnc.ImplicitUnion(implicitA, implicitB);
                DMesh3 newMesh = MeshMorphoLib.MeshClassFnc.GenerateMeshF(implicitUnionDone, num);
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

        public static DMesh3 ConvertDMesh(Rhino.Geometry.Mesh mesh)
        {
            List<g3.Vector3f> vertici;
            int[] triangoli;
            var vettori = MeshMorphoLib.MeshIntegration.DecomposeRhinoMesh(mesh, out triangoli, out vertici);

            var g3mesh = MeshMorphoLib.MeshClassIO.CreaMesh(vertici, triangoli, vettori);

            return g3mesh;
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
                return MorphoMeshEdit.Properties.Resources.ImplicitUnion;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("687f7769-5e92-4552-bac5-36e8d32eb082"); }
        }
    }
}