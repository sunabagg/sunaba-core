using System;
using Godot;
using System.Collections.Generic;
using System.Text.Json;

namespace Sunaba.Core;

public class AnimationData
{
    public string Name;
    public float Length;
    public string LoopMode;
    public object Tracks;
    
    public static AnimationData FromGodotAnimation(Animation gdAnimation, AnimationPlayer player)
    {
        AnimationData animationData = new();
        animationData.Name = gdAnimation.ResourceName;
        animationData.Length = gdAnimation.Length;
        animationData.LoopMode = gdAnimation.LoopMode.ToString();
        animationData.Tracks = ExtractTracks(gdAnimation, player);
        return animationData;
    }
    
    private Variant ConvertToVariant(object value, Animation.TrackType trackType)
    {
        if (value == null)
            return new Variant();

        if (value is Dictionary<string, object> valueDict)
        {
            if (valueDict.ContainsKey("X") && valueDict.ContainsKey("Y") && valueDict.ContainsKey("Z"))
            {
                // Assume Vector3
                var x = Convert.ToSingle(valueDict["X"]);
                var y = Convert.ToSingle(valueDict["Y"]);
                var z = Convert.ToSingle(valueDict["Z"]);
                return new Vector3(x, y, z);
            }
            else if (valueDict.ContainsKey("X") && valueDict.ContainsKey("Y") && valueDict.ContainsKey("Z") && valueDict.ContainsKey("W"))
            {
                // Assume Quaternion
                var x = Convert.ToSingle(valueDict["X"]);
                var y = Convert.ToSingle(valueDict["Y"]);
                var z = Convert.ToSingle(valueDict["Z"]);
                var w = Convert.ToSingle(valueDict["W"]);
                return new Quaternion(x, y, z, w);
            }
        }

        // For other types (e.g., float, int, string), return as Variant directly
        if (value is float floatValue)
            return floatValue;
        if (value is int intValue)
            return intValue;
        if (value is string stringValue)
            return stringValue;
        

        GD.PrintErr($"Unhandled value type for track: {value}");
        return new Variant(); // Default empty variant
    }

    
    public Animation ToGodotAnimation(AnimationPlayer player)
    {
        var animation = new Animation
        {
            ResourceName = Name,
            Length = Length,
            LoopMode = Enum.Parse<Animation.LoopModeEnum>(LoopMode)
        };

        if (Tracks is List<object> tracks)
        {
            foreach (var trackObj in tracks)
            {
                if (trackObj is not Dictionary<string, object> trackData)
                    continue;

                var trackType = Enum.Parse<Animation.TrackType>(trackData["Type"] as string ?? string.Empty);
                var trackPath = new NodePath(trackData["Path"] as string ?? string.Empty);
                var keyframes = trackData["Keyframes"] as List<object>;

                // Create the track
                var trackIndex = animation.AddTrack(trackType);
                animation.TrackSetPath(trackIndex, trackPath);

                if (keyframes != null)
                {
                    foreach (var keyframeObj in keyframes)
                    {
                        if (keyframeObj is not Dictionary<string, object> keyframeData)
                            continue;

                        var time = Convert.ToSingle(keyframeData["Time"]);
                        var value = ConvertToVariant(keyframeData["Value"], trackType);

                        // Add keyframe
                        animation.TrackInsertKey(trackIndex, time, value);
                    }
                }
            }
        }

        return animation;
    }

    
    public string ToJson()
    {
        var obj = new
        {
            Name = Name,
            Length = Length,
            LoopMode = LoopMode,
            Tracks = Tracks
        };
        var JsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        var json = JsonSerializer.Serialize(obj, JsonSerializerOptions);
        return json;
    }
    
    public static AnimationData FromJson(string json)
    {
        var obj = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        var animationData = new AnimationData
        {
            Name = obj["Name"] as string ?? string.Empty,
            Length = Convert.ToSingle(obj["Length"]),
            LoopMode = obj["LoopMode"] as string ?? string.Empty,
            Tracks = obj["Tracks"]
        };
        return animationData;
    }
    
    private static object ExtractTracks(Animation animation, AnimationPlayer player)
    {
        var tracks = new List<object>();

        for (int i = 0; i < animation.GetTrackCount(); i++)
        {
            var trackType = animation.TrackGetType(i);
            var trackNode = player.GetParent<Node>().GetNode(animation.TrackGetPath(i));
            if (trackNode == null)
            {
                GD.PrintErr($"Track node not found. Skipping track at index {i}.");
                continue; // Skip track if node not found
            }
            if (trackType == Animation.TrackType.Method)
            {
                GD.PrintErr($"Method tracks are not supported. Skipping track at index {i}.");
                continue; // Skip unsupported track
            }
            var nodeType = trackNode.GetType().ToString();
            nodeType = nodeType.Substring(nodeType.LastIndexOf('.') + 1);
            var trackData = new
            {
                Type = trackType.ToString(),
                Path = animation.TrackGetPath(i).ToString(),
                Node = nodeType,
                Keyframes = ExtractKeyframes(animation, i, trackType)
            };
            tracks.Add(trackData);
        }

        return tracks;
    }

    private static object ExtractKeyframes(Animation animation, int trackIndex, Animation.TrackType trackType)
    {
        var keyframes = new List<object>();

        for (int i = 0; i < animation.TrackGetKeyCount(trackIndex); i++)
        {
            var time = animation.TrackGetKeyTime(trackIndex, i);
            object value = null;

            // Extract value based on track type
            switch (trackType)
            {
                case Animation.TrackType.Method:
                    GD.PrintErr($"Method tracks are not supported. Skipping track at index {trackIndex}.");
                    continue; // Skip unsupported track

                default:
                    value = animation.TrackGetKeyValue(trackIndex, i);
                    var variant = (Variant)value;
                    
                    if (variant.AsVector3() != null)
                    {
                        var vec3 = variant.AsVector3();
                        value = new { X = vec3.X, Y = vec3.Y, Z = vec3.Z };
                    }
                    else if (variant.AsQuaternion() != null)
                    {
                        var quat = variant.AsQuaternion();
                        value = new { X = quat.X, Y = quat.Y, Z = quat.Z, W = quat.W };
                    }
                    else if (variant.AsInt32() != null)
                    {
                        value = variant.AsInt32();
                    }
                    else if (variant.AsInt64() != null)
                    {
                        value = variant.AsInt64();
                    }
                    else if (variant.AsSingle() != null)
                    {
                        value = variant.AsSingle();
                    }
                    else if (variant.AsDouble() != null)
                    {
                        value = variant.AsDouble();
                    }
                    else if (variant.AsString() != null)
                    {
                        value = variant.AsString();
                    }
                    else
                    {
                        throw new Exception($"Unhandled value type for track: {value.GetType().ToString()}");
                        value = null;
                    }
                    break;
            }

            keyframes.Add(new { Time = time, Value = value });
        }

        return keyframes;
    }

}