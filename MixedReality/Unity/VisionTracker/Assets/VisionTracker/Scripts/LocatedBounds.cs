using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LocatedBounds
{
    private const float RAD_TO_DEG = 180 / 3.14f;
    private const float IMG_HEIGHT = 0.4365625f;    // this is from physically measuring it TODO BETTER
    public const float HEAD_SIZE = 0.127f;          // this is a guess TODO BETTER

    /// <summary>
    /// Precondition: bounding box from face api on 2Dd image
    /// Parameters:
    /// - squareSidePixel: length retrieved from FaceAPI of the square bounding the person's face (pixels)
    /// - imgPixelWidth: resolution of 2D image width (pixels)
    /// - imgPixelHeight: resolution of 2D image height (pixels)
    /// - t: "unity_CameraProjection._m11" focal length?
    /// </summary>
    public static float CalculateDistance(float squareSidePixel, float imgPixelWidth, float imgPixelHeight, float t)
    {
        // Calculate real world measurements from pixel measurements
        var pixelsToMeters = IMG_HEIGHT / imgPixelHeight;
        var diagonal = Convert.ToSingle(Math.Sqrt((imgPixelHeight * imgPixelHeight) + (imgPixelWidth * imgPixelWidth))) * pixelsToMeters;
        var squareSide = squareSidePixel * pixelsToMeters;
        
        // Calculate distance between object and photo taken
        var fov = Mathf.Atan(1.0f / t) * 2.0f * RAD_TO_DEG; //  TODO see if these units are what we need
        var focalLength = 0.5f * (diagonal / Mathf.Tan(fov / 2));
        var objToPhotoDistance = HEAD_SIZE * focalLength / squareSide;

        return objToPhotoDistance;
    }
}