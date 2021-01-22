using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private Vector2Int coords;
    private Vector2 position;
    private float scale;
    GameObject go;
    SpriteRenderer sprite;

    public Vector2Int Coords { get => coords; set => coords = value; }

    public void Move(Vector2Int direction)
    {
        coords += direction;
        position += (Vector2)direction * scale;
        if (direction.x == 1)
        {
            go.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        if (direction.x == -1)
        {
            go.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
        }
        if (direction.y == 1)
        {
            go.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
        }
        if (direction.y == -1)
        {
            go.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
        }
        go.transform.position = position;
    }

    public void Destroy()
    {
        LabyCreator.DestroyGameObject(go);
    }



    public Player(Vector2Int startPosition, float scale, Sprite playerimg)
    {
        coords = startPosition;
        this.scale = scale;
        position = ((Vector2)startPosition + new Vector2(0.5f, 0.5f)) * scale;
        position.x -= 12.5f;
        position.y -= 14.5f;
        go = new GameObject();
        Vector3 pos = new Vector3(position.x, position.y, 1);
        go.transform.position = pos;
        sprite = go.AddComponent<SpriteRenderer>();
        sprite.sprite = playerimg;
        sprite.transform.localScale = new Vector3(scale, scale, scale);
    }
}
