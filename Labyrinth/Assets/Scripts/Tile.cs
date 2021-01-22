using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    private Vector2Int coords;
    private int type;
    private Vector2 position;
    private float scale;
    GameObject go;
    SpriteRenderer sprite;

    public int Type { get => type; }


    public void ChangeType(int newType)
    {
        type = newType;
    }

    public void Enabled(bool enabled)
    {
        sprite.enabled = enabled;
    }

    public void Destroy()
    {
        LabyCreator.DestroyGameObject(go);
    }

    public void Colour()
    {
        if (type == 0)
            sprite.color = Color.red;
        else if (type == 2)
            sprite.color = Color.yellow;
        else
            sprite.color = Color.green;
    }
    public void ResetColour()
    {
        if (type == 0 | type == 1)
            sprite.color = Color.white;
    }


    public Tile(Vector2Int coords, int type, float scale, Sprite texture)
    {
        this.coords = coords;
        this.type = type;
        this.scale = scale;
        position = (Vector2)coords * scale;
        position.x -= 12.5f;
        position.y -= 14.5f;
        //Debug.Log(position.x + ":" + position.y + "=" + type);
        go = new GameObject();
        Vector3 pos = new Vector3(position.x, position.y, 2);
        go.transform.position = pos;
        sprite = go.AddComponent<SpriteRenderer>();
        sprite.sprite = texture;
        sprite.transform.localScale = new Vector3(scale, scale, scale);
    }
}
