using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace SHProject.Ingame
{
    internal class FXNameList
    {
        public const string CharacterMove = "Prefabs/Effect/GridClick";
    }

    [AutoRegisterEvent]
    public class FXManager : MonoBehaviourBase, IPooledObject<ParticleSystem>
    {
        private const int maxPoolCount = 10;

        private Dictionary<string, GameObject> loadedFx = new Dictionary<string, GameObject>();
        private Dictionary<string, List<ParticleSystem>> _fxObjectPool = new Dictionary<string, List<ParticleSystem>>();

        public ParticleSystem GetPoolItem(string objName)
        {
            if (_fxObjectPool.ContainsKey(FXNameList.CharacterMove))
            {
                var poolList = _fxObjectPool[FXNameList.CharacterMove];
                if (poolList.Count > maxPoolCount)
                {
                    var obj = poolList
                        .Where(s => !s.gameObject.activeSelf)
                        .FirstOrDefault();

                    return obj;
                }
                else
                {
                    var resource = loadedFx[FXNameList.CharacterMove];
                    var inst = Instantiate<GameObject>(resource);
                    inst.transform.parent = this.transform;
                    inst.transform.localPosition = Vector3.zero;
                    inst.transform.localRotation = Quaternion.identity;

                    var particle = inst.GetComponent<ParticleSystem>();
                    poolList.Add(particle);

                    return particle;
                }
            }
            else
            {
                var resource = Resources.Load<GameObject>(FXNameList.CharacterMove);
                loadedFx.Add(FXNameList.CharacterMove, resource);

                var inst = Instantiate<GameObject>(resource);
                inst.transform.parent = this.transform;
                inst.transform.localPosition = Vector3.zero;
                inst.transform.localRotation = Quaternion.identity;

                var particle = inst.GetComponent<ParticleSystem>();
                var list = new List<ParticleSystem>();
                list.Add(particle);
                _fxObjectPool.Add(FXNameList.CharacterMove, list);

                return particle;
            }
        }

        private void PlayFX(ParticleSystem particle, Locate locate)
        {
            particle.gameObject.SetActive(true);

            Vector3 mapPos = Map.Instance.GetMapPosition(locate);
            mapPos.y = 0.1f;
            particle.transform.localPosition = mapPos;
            particle.Play();
        }

        [EventMethod(EventEnum.CharacterMove)]
        public void OnCharacterMoveEffect(object sender, EventArgs args)
        {
            TValueEventArgs<PhotonPlayer, Locate> eventArgs = args as TValueEventArgs<PhotonPlayer, Locate>;
            if (eventArgs.arg1.ID != PhotonNetwork.player.ID) // Is Other Player
                return;

            var effect = GetPoolItem(FXNameList.CharacterMove);
            PlayFX(effect, eventArgs.arg2);
        }
    }
}