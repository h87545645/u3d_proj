using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class LandingState : IBaseState
{
    private FragHero _fragHore;
    public LandingState(FragHero frag)
    {
        _fragHore = frag;
        _fragHore.fragAnim.SetBool("landing", true);
        _fragHore.heroRigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX;
        // Vector3 cur = new Vector3(_fragHore.heroRigidbody2D.transform.position.x,
        //     _fragHore.heroRigidbody2D.transform.position.y, _fragHore.heroRigidbody2D.transform.position.z);
        // RaycastHit2D hit = Physics2D.Raycast(cur, Vector2.down, 0.11f + _fragHore.collider2D.size.y / 2 * _fragHore.transform.localScale.y, 1 << 3);

        // Vector3Int cellPos = _fragHore.tilemap.WorldToCell(_fragHore.heroRenderer.transform.position);
        // TileBase cell = _fragHore.tilemap.GetTile(cellPos);

        _fragHore.LastPosition = _fragHore.heroRenderer.transform.position;
        
        Debug.Log("------------------------FragHore in LandingState~!");
    }

    public void Update()
    {

    }

    public void HandleInput()
    {

    }
}
