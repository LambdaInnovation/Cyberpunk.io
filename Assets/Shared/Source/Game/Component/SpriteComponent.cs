using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

[Game]
public class SpriteComponent : IComponent {

	public Sprite sprite;
	public bool dynamic;
	
	public SpriteRenderer spriteRenderer;

}
