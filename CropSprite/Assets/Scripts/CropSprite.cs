using UnityEngine;
using System.Collections;

public class CropSprite : MonoBehaviour 
{
//	Reference for sprite which will be cropped and it has BoxCollider or BoxCollider2D
	public GameObject spriteToCrop;

	private Vector3 startPoint, endPoint;
	private bool isMousePressed;
//	For sides of rectangle. Rectangle that will display cropping area
	private LineRenderer leftLine, rightLine, topLine, bottomLine;

	void Start () 
	{
		isMousePressed = false;
//		Instantiate rectangle sides
		leftLine = createAndGetLine("LeftLine");
		rightLine = createAndGetLine("RightLine");
		topLine = createAndGetLine("TopLine");
		bottomLine = createAndGetLine("BottomLine");
	}
//	Creates line through LineRenderer component
	private LineRenderer createAndGetLine (string lineName)
	{
		GameObject lineObject = new GameObject(lineName);
		LineRenderer line = lineObject.AddComponent<LineRenderer>();
		line.SetWidth(0.03f,0.03f);
		line.SetVertexCount(2);
		return line;
	}
	void Update () 
	{
		if(Input.GetMouseButtonDown(0) && isSpriteTouched(spriteToCrop))
		{
			isMousePressed = true;
			startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}
		else if(Input.GetMouseButtonUp(0))
		{
			if(isMousePressed)
				cropSprite();	
			isMousePressed = false;
		}
		if(isMousePressed && isSpriteTouched(spriteToCrop))
		{
			endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			drawRectangle();
		}
	}
//	Following method draws rectangle that displays cropping area
	private void drawRectangle()
	{
		leftLine.SetPosition(0, new Vector3(startPoint.x, endPoint.y, 0));
		leftLine.SetPosition(1, new Vector3(startPoint.x, startPoint.y, 0));

		rightLine.SetPosition(0, new Vector3(endPoint.x, endPoint.y, 0));
		rightLine.SetPosition(1, new Vector3(endPoint.x, startPoint.y, 0));

		topLine.SetPosition(0, new Vector3(startPoint.x, startPoint.y, 0));
		topLine.SetPosition(1, new Vector3(endPoint.x, startPoint.y, 0));

		bottomLine.SetPosition(0, new Vector3(startPoint.x, endPoint.y, 0));
		bottomLine.SetPosition(1, new Vector3(endPoint.x, endPoint.y, 0));
	}
	//	Following method crops as per displayed cropping area
	private void cropSprite()
	{
//		Calculate topLeftPoint and bottomRightPoint of drawn rectangle
		Vector3 topLeftPoint = startPoint, bottomRightPoint=endPoint;
		if((startPoint.x > endPoint.x))
		{
			topLeftPoint = endPoint;
			bottomRightPoint = startPoint;
		}
		if(bottomRightPoint.y > topLeftPoint.y)
		{
			float y = topLeftPoint.y;
			topLeftPoint.y = bottomRightPoint.y;
			bottomRightPoint.y = y;
		}

		SpriteRenderer spriteRenderer = spriteToCrop.GetComponent<SpriteRenderer>();
		Sprite spriteToCropSprite = spriteRenderer.sprite;
		Texture2D spriteTexture = spriteToCropSprite.texture;
		Rect spriteRect = spriteToCrop.GetComponent<SpriteRenderer>().sprite.textureRect;

		int pixelsToUnits = 100; // It's PixelsToUnits of sprite which would be cropped

//		Crop sprite

		GameObject croppedSpriteObj = new GameObject("CroppedSprite");
		Rect croppedSpriteRect = spriteRect;
		croppedSpriteRect.width = (Mathf.Abs(bottomRightPoint.x - topLeftPoint.x)*pixelsToUnits)* (1/spriteToCrop.transform.localScale.x);
		croppedSpriteRect.x = (Mathf.Abs(topLeftPoint.x - (spriteRenderer.bounds.center.x-spriteRenderer.bounds.size.x/2)) *pixelsToUnits)* (1/spriteToCrop.transform.localScale.x);
		croppedSpriteRect.height = (Mathf.Abs(bottomRightPoint.y - topLeftPoint.y)*pixelsToUnits)* (1/spriteToCrop.transform.localScale.y);
		croppedSpriteRect.y = ((topLeftPoint.y - (spriteRenderer.bounds.center.y - spriteRenderer.bounds.size.y/2))*(1/spriteToCrop.transform.localScale.y))* pixelsToUnits - croppedSpriteRect.height;//*(spriteToCrop.transform.localScale.y);
		Sprite croppedSprite = Sprite.Create(spriteTexture, croppedSpriteRect, new Vector2(0,1), pixelsToUnits);
		SpriteRenderer cropSpriteRenderer = croppedSpriteObj.AddComponent<SpriteRenderer>();	
		cropSpriteRenderer.sprite = croppedSprite;
		topLeftPoint.z = -1;
		croppedSpriteObj.transform.position = topLeftPoint;
		croppedSpriteObj.transform.parent = spriteToCrop.transform.parent;
		croppedSpriteObj.transform.localScale = spriteToCrop.transform.localScale;
		Destroy(spriteToCrop);
	}

//	Following method checks whether sprite is touched or not. There are two methods for simple collider and 2DColliders. you can use as per requirement and comment another one.

//	For simple 3DCollider
//	private bool isSpriteTouched(GameObject sprite)
//	{
//		RaycastHit hit;
//		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
//		if (Physics.Raycast (ray, out hit)) 
//		{
//			if (hit.collider != null && hit.collider.name.Equals (sprite.name)) 
//				return true;
//		}
//		return false;
//	}

//	For 2DCollider
	private bool isSpriteTouched(GameObject sprite)
	{
		Vector3 posFor2D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		RaycastHit2D hit2D = Physics2D.Raycast(posFor2D, Vector2.zero);
		if (hit2D != null && hit2D.collider != null)
		{
			if(hit2D.collider.name.Equals(sprite.name))
				return true;
		}
		return false;
	}
}
