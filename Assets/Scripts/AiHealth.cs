using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiHealth : Health
{
    AiAgent agent;
    UIHealthBar healthbar;
    [SerializeField] Material dissolveShader;
    [SerializeField] float dissolveTime = 3;

    protected override void OnStart()
    {
        agent = GetComponent<AiAgent>();
        healthbar = GetComponentInChildren<UIHealthBar>();

    }
    protected override void OnDamage(Vector3 direction)
    {
        healthbar.SetHealthBarPercentage(currentHealth / maxHealth);
    }

    protected override void OnDeath(Vector3 direction)
    {
        StartCoroutine(Dissolve());
        AiDeathState deathState = agent.stateMachine.GetState(AiStateId.Death) as AiDeathState;
        deathState.direction = direction;
        agent.stateMachine.ChangeState(AiStateId.Death);
    }

    IEnumerator Dissolve()
    {
        skinnedMeshRenderer.material = Instantiate<Material>(dissolveShader);
        float lerp = 0;
        float timer = 0;
        while (lerp < 1)
        {
            lerp = timer / dissolveTime;
            skinnedMeshRenderer.material.SetFloat("_Slider", lerp);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }
}
