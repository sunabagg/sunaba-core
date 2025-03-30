using Godot;

namespace Sunaba.Core;

[GlobalClass]
public partial class TextureLoader : RefCounted
{
	private IoInterface _ioInterface;
	
	public TextureLoader(IoInterface ioInterface) 
	{
		_ioInterface = ioInterface;
	}
	
	public Texture2D LoadTexture(string path)
	{
		// Load the texture from the given path using the IoInterface
		// This is a placeholder implementation, you would need to implement the actual loading logic
		if (_ioInterface.FileExists(path))
		{
			Image image = new Image();
			byte[] imageBytes = _ioInterface.LoadBytes(path);
			if (path.EndsWith(".png"))
				image.LoadPngFromBuffer(imageBytes);
			else if (path.EndsWith(".jpg") || path.EndsWith(".jpeg"))
				image.LoadJpgFromBuffer(imageBytes);
			else if (path.EndsWith(".bmp"))
				image.LoadBmpFromBuffer(imageBytes);
			else if (path.EndsWith(".webp"))
				image.LoadWebpFromBuffer(imageBytes);
			else if (path.EndsWith(".tga"))
				image.LoadTgaFromBuffer(imageBytes);
			else if (path.EndsWith(".ktx"))
				image.LoadKtxFromBuffer(imageBytes);
			else if (path.EndsWith(".svg"))
				image.LoadSvgFromBuffer(imageBytes);
			else
			{
				GD.PrintErr($"Unsupported image format: {path}");
				return null; // Return null if the format is not supported
			}
			ImageTexture texture = ImageTexture.CreateFromImage(image);
			return texture;
		}
		
		GD.PrintErr($"Texture not found at path: {path}");
		return null; // Return null if the texture could not be loaded
	}
}
