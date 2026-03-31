using UnityEngine;

public class PokemonSubAnimator : MonoBehaviour
{

    public BattleManager bm;
    //whoops. Didn't realize you can't access a function in a different script from the animators object. Not redoing the animations now fuck that
    public void EndAnimation()
    {
        bm.OnAnimationEnd(transform.gameObject.CompareTag("PlayerPokemon"));
    }

    //oh. well. this sucks. i dont like unitys animation system
    //sowwy Nathan
    public void InstaitateZs()
    {
        bm.StartCoroutine(bm.RestAnimationCR());        
    }

    public void InstantiateFakeZs()
    {
        bm.StartCoroutine(bm.FakeRestAnimationCR());
    }

        public void InstantiateEnemyZs()
    {
        bm.StartCoroutine(bm.RestAnimationEnemyCR());
    }

}