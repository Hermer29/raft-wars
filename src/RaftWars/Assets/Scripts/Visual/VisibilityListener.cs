using System;
using System.Collections.Generic;
using Common;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class VisibilityListener : MonoBehaviour
{
    public static Dictionary<EnemyHud, List<VisibilityListener>> _visibility = new Dictionary<EnemyHud, List<VisibilityListener>>();
    private Renderer _renderer;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    public bool IsVisible()
    {
        return _renderer.isVisible;
    }

    private void OnBecameVisible()
    {
        SearchForHud().BecameVisible();
    }

    private void OnBecameInvisible()
    {
        SearchForHud().BecameInvisible();
    }

    private EnemyHud SearchForHud()
    {
        var current = transform;
        FighterRaft raft;
        while (current.TryGetComponent(out raft) == false)
        {
            current = current.parent;
        }

        var hud = raft.GetHud();
        if(_visibility.ContainsKey(hud) == false)
        {
            _visibility.Add(hud, new List<VisibilityListener>());
        }

        if (_visibility[hud].Contains(this) == false)
        {
            _visibility[hud].Add(this);
        }

        return hud;
    }

    private void OnDestroy()
    {
        _visibility[SearchForHud()].Remove(this);
    }
}