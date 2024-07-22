using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VectorGraphics;
using UnityEngine;


public class DroptSvgLoader
{
    // Uses the Unity SVG Lib to import an SVG Sprite at runtime
    public static Sprite CreateSvgSprite(string data, Vector2 customPivot, bool preserveViewport = true)
    {
        if (string.IsNullOrEmpty(data))
            return null;

        try
        {
            ViewportOptions viewportOptions = ViewportOptions.PreserveViewport;
            if (!preserveViewport)
            {
                viewportOptions = ViewportOptions.DontPreserve;
            }

            //float dpi = 0f;
            //float pixelsPerUnit = 1.0f;
            //int windowWidth = 0;
            //int windowHeight = 0;

            /*
            float stepDistance = 100.0f;
            float samplingStepSize = 0.01f;
            float maxCoordDeviation = 0.5f; //0.01
            float maxTanAngleDeviation = 0.1f;
            */

            float stepDistance = 10.0f;
            float samplingStepSize = 100.0f;
            float maxCoordDeviation = float.MaxValue;
            float maxTanAngleDeviation = Mathf.PI * 0.5f;

            float svgPixelsPerUnit = 36f;

            VectorUtils.Alignment alignment = VectorUtils.Alignment.Center;

            if (customPivot != Vector2.zero)
                alignment = VectorUtils.Alignment.Custom;

            //Vector2 customPivot = Vector2.zero;

            ushort gradientResolution = 256;
            bool flipYAxis = true;

            var sceneInfo = SVGParser.ImportSVG(new StringReader(data), viewportOptions, 0, 1, 64, 64);

            if (sceneInfo.Scene == null || sceneInfo.Scene.Root == null)
                throw new Exception("Wowzers!");


            var tessOptions = new VectorUtils.TessellationOptions()
            {
                StepDistance = stepDistance,
                SamplingStepSize = samplingStepSize,
                MaxCordDeviation = maxCoordDeviation,
                MaxTanAngleDeviation = maxTanAngleDeviation,
            };

            var geoms = VectorUtils.TessellateScene(sceneInfo.Scene, tessOptions, sceneInfo.NodeOpacity);

            if (geoms.Count == 0)
            {
                return null;
            }

            Sprite sprite = null;
            {
                sprite = VectorUtils.BuildSprite(geoms, sceneInfo.SceneViewport, svgPixelsPerUnit,
                    alignment, customPivot, gradientResolution, flipYAxis);
            }

            return sprite;

        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        return null;
    }
}
