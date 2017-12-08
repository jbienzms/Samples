using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LocatedBounds
{
    private const float RAD_TO_DEG = 180 / 3.14f;
    private const double IMG_WIDTH = 1;             // meters; TODO find real value for this
    public const float HEAD_SIZE = 0.25f;           // meters; TODO find a way to get a more reliable reading

    /// <summary>
    /// Precondition: bounding box from face api on 2Dd image
    /// Parameters:
    /// - squareSidePixel: length retrieved from FaceAPI of the square bounding the person's face (pixels)
    /// - imgPixelWidth: resolution of 2D image width (pixels)
    /// - imgPixelHeight: resolution of 2D image height (pixels)
    /// - t: "unity_CameraProjection._m11" focal length?
    /// </summary>
    public static double CalculateDistance(double squareSidePixel, int imgPixelWidth, int imgPixelHeight, float t)
    {
        // Calculate real world measurements from pixel measurements
        var pixelsToMeters = IMG_WIDTH / imgPixelWidth;
        var diagonal = Math.Sqrt(imgPixelHeight * imgPixelWidth * imgPixelHeight * imgPixelWidth) * pixelsToMeters;
        var squareSide = squareSidePixel * pixelsToMeters;

        // Calculate distance between object and photo taken
        var fov = Mathf.Atan(1.0f / t) * 2.0f * RAD_TO_DEG; //  TODO see if these units are what we need
        var focalLength = 0.5 * diagonal / Mathf.Tan(fov);
        var objToPhotoDistance = HEAD_SIZE * focalLength / squareSide;

        return objToPhotoDistance;
    }
}