using System;
using UnityEngine;

public class FallTree : MonoBehaviour
{
	[Header("FallTree")]
	public Vector3 Dir = default(Vector3);
	public Animator anim;
	public float lifeTime = 2f;

	[Header("Audio")]
	public AudioClip clip;
	public AudioSource audio;

	private GameObject CollisionTrigger;
	private bool isDestroyed;
	public bool isMasterServerObject;
	public static Action<FallTree, Vector3, float> OnPlayCollapseAnimation;

	public Collider coll;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Tank")
		{
			coll.enabled = false;
			Transform fen = transform.GetChild(0).Find("Tank");
			CollisionTrigger = ((fen != null) ? fen.gameObject : null);
			if (!isDestroyed)
			{
				TankControll componentInChildren = other.transform.root.GetComponent<TankControll>();
				if (componentInChildren != null)
				{
					Vector3 velocity = componentInChildren.rb.velocity;
					float maxMoveSpeed = componentInChildren.maxMoveSpeed;
					PlayCollapseAnimation(velocity, maxMoveSpeed);
				}
			}
		}
	}

	public void PlayCollapseAnimation(Vector3 Dirs, float Velocity)
	{
		Dir = Dirs;
		if (!isMasterServerObject)
		{
			bool activ = Vector3.Dot(Dir, base.transform.forward) < 0f;
			if (activ)
			{
				Dirs = -Dirs;
			}
			transform.rotation = Quaternion.LookRotation(Dirs);
			anim.enabled = true;
			if (Velocity > 10f)
			{
				if (activ)
				{
					anim.Play("ReverseFallTree");
				}
				else
				{
					anim.Play("FallTree");
				}
			}
			else if (activ)
			{
				anim.Play("FallTreeReverse");
			}
			else
			{
				anim.Play("TreeFall");
			}
		}
		else
		{
			CollisionTrigger.SetActive(false);
		}
		Action<FallTree, Vector3, float> onPlayFallTree= FallTree.OnPlayCollapseAnimation;
		if (onPlayFallTree!= null)
		{
			onPlayFallTree(this, Dirs, Velocity);
		}
		DestroyTree();
	}

	private void DestroyTree()
	{
		audio.PlayOneShot(clip);
		Destroy(gameObject, lifeTime);
	}
}
