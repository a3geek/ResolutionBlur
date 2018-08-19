using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ResolutionBlur
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public class Blur : MonoBehaviour
    {
        public const int MaxIteratin = 10;

        [SerializeField]
        private bool validity = true;
        [SerializeField, Range(1, MaxIteratin)]
        private int iteration = 5;

        private RenderTexture[] rts = new RenderTexture[MaxIteratin];
        
        
        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if(this.validity == false)
            {
                Graphics.Blit(source, destination);
                return;
            }

            var width = source.width;
            var height = source.height;
            
            var i = 0;

            // Step downsampling.
            for(i = 0; i < this.iteration; i++)
            {
                width /= 2;
                height /= 2;
                if(width < 2 || height < 2)
                {
                    break;
                }

                rts[i] = RenderTexture.GetTemporary(width, height, 0, source.format);
                Graphics.Blit(i == 0 ? source : rts[i - 1], rts[i]);
            }

            // Step upsampling.
            for(i -= 1; i > 0; i--)
            {
                Graphics.Blit(rts[i], rts[i - 1]);
                rts[i].Release();
            }

            Graphics.Blit(rts[0], destination);
            rts[0].Release();
        }
    }
}
