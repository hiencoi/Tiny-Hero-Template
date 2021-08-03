using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UDEV.AI2D;
using UDEV.SPM;

namespace UDEV.TinyHero {
    [RequireComponent(typeof(aiBase))]
    public class EnemyAdditions : MonoBehaviour
    {
        [Header("Collectale Spawn Settings:")]
        [Range(0f, 1f)]
        public float commonSpawnRate;
        public int commonAmount = 5;
        [Range(0f, 1f)]
        public float rareSpawnRate;
        public int rareAmount = 1;
        [Range(0f, 1f)]
        public float epicSpawnRate;
        public int epicAmount = 1;
        public Vector3 spawnOffset;

        [Header("Game Score And Xp Bonus Settings:")]
        public Stat minScoreBonus;
        public Stat maxScoreBonus;
        public Stat minXpBonus;
        public Stat maxXpBonus;

        [Header("VFX:")]
        [PoolerKeys(target = PoolerTarget.VFX)]
        public string deathVfx;

        [Header("UI:")]
        [PoolerKeys(target = PoolerTarget.AI)]
        public string healthBar;
        public Vector3 hpBarYOffset;
        public Vector3 hbBarScale = Vector3.one;

        public int XpBonus { get => Random.Range(
            minXpBonus.GetIntValue(), maxXpBonus.GetIntValue()); }

        public int ScoreBonus { get => Random.Range(
            minScoreBonus.GetIntValue(), maxScoreBonus.GetIntValue()); }

        aiBase m_ai;

        ImageFilled m_healthBar; // Health bar UI

        private void Awake()
        {
            m_ai = GetComponent<aiBase>();

            if(m_ai)
                AddSkillEvents();
        }

        private void OnEnable()
        {
            if (m_ai)
            {
                CreateHealthBarUI();

                m_ai.OnDead.AddListener(() => OnDeadEvent());
            }
        }

        private void Update()
        {
            //Update position and vale of health bar UI
            if (m_healthBar)
            {
                m_healthBar.UpdateValue((float)m_ai.CurHealth, (float)m_ai.health.GetValue());
            }
        }

        void LateUpdate()
        {
            //Update position and vale of health bar UI
            if (m_healthBar)
            {
                FlipHpBarOffset();

                m_healthBar.transform.position = transform.position + hpBarYOffset;
            }
        }

        void FlipHpBarOffset()
        {
            switch (m_ai.facing)
            {
                case aiBase.Facing.LEFT:
                    if (transform.localScale.x < 0)
                    {
                        if (hpBarYOffset.x < 0)
                            hpBarYOffset = new Vector3(hpBarYOffset.x * -1, hpBarYOffset.y, hpBarYOffset.z);
                    }
                    else if (transform.localScale.x > 0)
                    {
                        if (hpBarYOffset.x > 0)
                            hpBarYOffset = new Vector3(hpBarYOffset.x * -1, hpBarYOffset.y, hpBarYOffset.z);
                    }
                    break;
                case aiBase.Facing.RIGHT:
                    if (transform.localScale.x > 0)
                    {
                        if (hpBarYOffset.x < 0)
                            hpBarYOffset = new Vector3(hpBarYOffset.x * -1, hpBarYOffset.y, hpBarYOffset.z);
                    }
                    else if (transform.localScale.x < 0)
                    {
                        if (hpBarYOffset.x > 0)
                            hpBarYOffset = new Vector3(hpBarYOffset.x * -1, hpBarYOffset.y, hpBarYOffset.z);
                    }
                    break;
            }
        }

        void CreateHealthBarUI()
        {
            //Spawn and update health bar UI
            GameObject hpBar = PoolersManager.Ins.Spawn(PoolerTarget.AI, healthBar, transform.position, Quaternion.identity);

            if (hpBar)
            {
                hpBar.transform.localScale = hbBarScale;
                m_healthBar = hpBar.GetComponent<ImageFilled>();

                if (m_healthBar)
                {
                    m_healthBar.UpdateValue((float)m_ai.CurHealth, (float)m_ai.health.GetValue());
                    m_healthBar.Root = transform;
                }
            }
        }

        void OnDeadEvent()
        {
            GameManager.Ins.Player.AddXp(XpBonus);

            GameManager.Ins.Score += ScoreBonus;

            GameManager.Ins.EnemiesKilled++;

            //Spawn death vfx if we have

            GameObject deathVfxClone = null;

            deathVfxClone = PoolersManager.Ins.Spawn(PoolerTarget.VFX, deathVfx, transform.position, Quaternion.identity);

            aiSpawned spawnerComp = GetComponent<aiSpawned>();

            if (spawnerComp)
            {
                spawnerComp.Dead();
            }

            SpawnCollectables(m_ai.transform.position + spawnOffset);

            RemoveModifiers();

            if (m_healthBar) m_healthBar.Show(false);
        }

        void BonusesUp(string val)
        {
            if (!string.IsNullOrEmpty(val))
            {
                int b = int.Parse(val);

                if(b > 0)
                {
                    minScoreBonus.AddModifier(b);
                    minXpBonus.AddModifier(b);
                    maxScoreBonus.AddModifier(b);
                    maxXpBonus.AddModifier(b);
                }
            }
        }

        void HealthUp(string health)
        {
            if (m_ai && !string.IsNullOrEmpty(health))
            {
                int h = int.Parse(health);

                if (h > 0)
                    m_ai.health.AddModifier(h);

                m_ai.LoadData();
            }
        }

        void DamageUp(string damage)
        {
            if (m_ai && !string.IsNullOrEmpty(damage))
            {
                int d = int.Parse(damage);

                if (d > 0)
                {
                    m_ai.bodyDamage.AddModifier(d);

                    if (m_ai.featuresManager)
                        m_ai.featuresManager.AddModifierToAll(d);
                }

                m_ai.LoadData();
            }
        }

        void RemoveModifiers()
        {
            minScoreBonus.RemoveModifiers();
            minXpBonus.RemoveModifiers();
            maxScoreBonus.RemoveModifiers();
            maxXpBonus.RemoveModifiers();
        }

        /// <summary>
        /// Spawn Collectables when AI die
        /// </summary>
        /// <param name="position">Position</param>
        public void SpawnCollectables(Vector3 position)
        {
            float curSpawnRate = Random.Range(0f, 1f);

            if (curSpawnRate <= commonSpawnRate)
                CollectablesManager.Ins.SpawnCommonCollectable(position, commonAmount);

            if (curSpawnRate <= rareSpawnRate)
                CollectablesManager.Ins.SpawnRareCollectable(position, rareAmount);

            if (curSpawnRate <= epicSpawnRate)
                CollectablesManager.Ins.SpawnEpicCollectable(position, epicAmount);
        }

        #region PLAYER_SKILL_EVENTS
        protected virtual void AddSkillEvents()
        {
            List<Skill> activeSkills = GameManager.Ins.Player.skillsManager.Skills;

            if (activeSkills != null && activeSkills.Count > 0)
            {
                foreach (Skill skill in activeSkills)
                {
                    if (skill)
                    {
                        if (skill is FreezeEnemies)
                        {
                            skill.OnTriggerBegin.AddListener(() => Freeze(true));
                            skill.OnTriggerEnd.AddListener(() => Freeze(false));
                        }
                    }
                }
            }
        }

        public void Freeze(bool isOn)
        {
            if (m_ai == null) return;

            m_ai.IsFreeze = isOn;
            if(m_ai.rb)
                m_ai.rb.velocity = m_ai.IsFreeze ? Vector2.zero : m_ai.rb.velocity;
            if (m_ai.anim)
                m_ai.anim.enabled = isOn ? false : true;
            if (m_ai.featuresManager)
                m_ai.featuresManager.enabled = isOn ? false : true;
        }
        #endregion

        private void OnDrawGizmos()
        {
            if (!string.IsNullOrEmpty(healthBar))
            {
                Gizmos.DrawIcon(transform.position + hpBarYOffset, "HPBar_Icon.png", true);
            }
        }
    }
}
