using System;
using System.Collections.Generic;
using System.Linq;
using DcmAnonymize.Blanking;

namespace DcmAnonymize;

public record AnonymizationOptions(List<RectangleToBlank> RectanglesToBlank)
{
    public static AnonymizationOptions Parse(IEnumerable<string>? rectanglesToBlank)
    {
        if (rectanglesToBlank == null)
        {
            return new AnonymizationOptions(new List<RectangleToBlank>());
        }

        return new AnonymizationOptions(rectanglesToBlank.Select(ParseRectangleToBlank).ToList());
    }

    private static RectangleToBlank ParseRectangleToBlank(string rectangleToBlank)
    {
        ArgumentException.ThrowIfNullOrEmpty(rectangleToBlank);
        var coordinates = rectangleToBlank.Split("->", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (coordinates.Length != 2)
        {
            throw new ArgumentException("Invalid rectangle to blank argument: " + rectangleToBlank);
        }
        var (x1, y1) = ParseCoordinate(coordinates[0]);
        var (x2, y2) = ParseCoordinate(coordinates[1]);
        return new RectangleToBlank(x1, y1, x2, y2);
    }

    private static (int X, int Y) ParseCoordinate(string coordinate)
    {
        if (string.IsNullOrEmpty(coordinate))
        {
            throw new ArgumentException("Empty coordinate in rectangle to blank");
        }
        var coordinateValues = coordinate.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (coordinateValues.Length != 2)
        {
            throw new ArgumentException("Invalid coordinate in rectangle to blank: " + coordinate);
        }
        if (!int.TryParse(coordinateValues[0], out int x))
        {
            throw new ArgumentException("Invalid coordinate x value in rectangle to blank: " + coordinate);
        }
        if (!int.TryParse(coordinateValues[1], out int y))
        {
            throw new ArgumentException("Invalid coordinate y value in rectangle to blank: " + coordinate);
        }
        return (x, y);
    }
}
