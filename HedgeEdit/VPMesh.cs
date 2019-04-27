﻿using HedgeLib.Models;
using SharpDX.Direct3D11;
using System;
using Buffer = SharpDX.Direct3D11.Buffer;
using static HedgeEdit.Data;
using HedgeEdit.D3D;
using SharpDX;
using SharpDX.DXGI;

namespace HedgeEdit
{
    public struct VPMesh : IDisposable
    {
        // Variables/Constants
        public string MaterialName;
        public int TriangleCount;
        public Mesh.Slots Slot;
        private Buffer indexBuffer;
        private VertexBufferBinding binding;

        // Constructors
        public VPMesh(Buffer indexBuffer, VertexBufferBinding binding, Mesh mesh)
        {
            this.indexBuffer = indexBuffer ??
                throw new ArgumentNullException("indexBuffer");

            this.binding = binding;
            MaterialName = mesh.MaterialName;
            TriangleCount = mesh.Triangles.Length;
            Slot = mesh.Slot;
        }

        public VPMesh(Buffer indexBuffer, VertexBufferBinding binding,
            string materialName, int triangleCount, Mesh.Slots slot)
        {
            this.indexBuffer = indexBuffer ??
                throw new ArgumentNullException("indexBuffer");

            this.binding = binding;
            MaterialName = materialName;
            TriangleCount = triangleCount;
            Slot = slot;
        }

        // Methods
        public void Bind(bool skipMaterial = false)
        {
            if (!skipMaterial)
            {
                // Get the material
                var mat = (string.IsNullOrEmpty(MaterialName) ||
                    !Materials.ContainsKey(MaterialName)) ?
                    DefaultMaterial : Materials[MaterialName];

                // Set the shader
                Viewport.CurrentPShader = (PixelShaders.ContainsKey(mat.ShaderName)) ?
                    PixelShaders[mat.ShaderName] : PixelShaders["Default"];

                // Get the texture
                string texName = (mat.Texset.Textures.Count > 0) ?
                    mat.Texset.Textures[0].TextureName : null;

                var tex = (string.IsNullOrEmpty(texName) ||
                    !Textures.ContainsKey(texName)) ?
                    DefaultTexture : Textures[texName];

                // Update Constant Buffers
                if (Viewport.RenderMode == Viewport.RenderModes.HedgehogEngine2)
                {
                    // Update CBMaterialAnimation Constant Buffer
                    // TODO: Get material animation from material and use that
                    Buffers.HE2.CBMaterialAnimation.Data.diffuse_color =
                        new Vector4(1, 1, 1, 1);

                    Buffers.HE2.CBMaterialAnimation.Data.emissive_color = new Vector4(
                        0.25f, 0.25f, 0.25f, 1);
                    Buffers.HE2.CBMaterialAnimation.Data.ambient_color = Vector4.One;
                    Buffers.HE2.CBMaterialAnimation.Update();
                    Buffers.HE2.CBMaterialAnimation.PSSetConstantBuffer(4);

                    // TODO: Don't update CBMaterialStatic every frame
                    Buffers.HE2.CBMaterialStatic.Data.Luminance = Vector4.One;

                    Buffers.HE2.CBMaterialStatic.Update();
                    Buffers.HE2.CBMaterialStatic.PSSetConstantBuffer(3);
                }

                // Set Texture
                Viewport.Context.PixelShader.SetShaderResource(0, tex);
            }

            // Bind our Buffers
            Viewport.InputAssembler.SetVertexBuffers(0, binding);
            Viewport.InputAssembler.SetIndexBuffer(indexBuffer, Format.R32_UInt, 0);
        }

        public void Draw()
        {
            Viewport.Context.DrawIndexed(TriangleCount, 0, 0);
        }

        public void Dispose()
        {
            Utilities.Dispose(ref indexBuffer);

            if (binding.Buffer != null)
                binding.Buffer.Dispose();
        }
    }
}