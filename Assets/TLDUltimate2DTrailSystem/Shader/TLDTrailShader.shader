Shader "TLD/Ultimate 2D Trail"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
        _GlowIntensity ("Glow Intensity", Range(0, 5)) = 1
        _GlowColor ("Glow Color", Color) = (1,1,1,1)
        _Softness ("Edge Softness", Range(0, 1)) = 0.5
        
        [Toggle] _UseGlow ("Use Glow", Float) = 0
        [Toggle] _UseDashed ("Use Dashed", Float) = 0
        _DashLength ("Dash Length", Float) = 0.2
        _DashGap ("Dash Gap", Float) = 0.1
        
        [Toggle] _UseSparkles ("Use Sparkles", Float) = 0
        _SparkleSize ("Sparkle Size", Float) = 0.05
        _SparkleSpeed ("Sparkle Speed", Float) = 1
        
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend Mode", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend Mode", Float) = 1
        [Enum(Off,0,On,1)] _ZWrite ("Z Write", Float) = 0
    }
    
    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
        }
        
        LOD 100
        
        Blend [_SrcBlend] [_DstBlend]
        ZWrite [_ZWrite]
        Cull Off
        Lighting Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float3 worldPos : TEXCOORD2;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _GlowColor;
            float _GlowIntensity;
            float _Softness;
            
            float _UseGlow;
            float _UseDashed;
            float _DashLength;
            float _DashGap;
            float _UseSparkles;
            float _SparkleSize;
            float _SparkleSpeed;
            
            // Hash function for sparkles
            float hash(float2 p)
            {
                float3 p3 = frac(float3(p.xyx) * 0.13);
                p3 += dot(p3, p3.yzx + 3.333);
                return frac((p3.x + p3.y) * p3.z);
            }
            
            // Noise function for sparkles
            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                
                float2 u = f * f * (3.0 - 2.0 * f);
                
                return lerp(lerp(hash(i + float2(0.0, 0.0)),
                                hash(i + float2(1.0, 0.0)), u.x),
                           lerp(hash(i + float2(0.0, 1.0)),
                                hash(i + float2(1.0, 1.0)), u.x), u.y);
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the texture
                fixed4 texColor = tex2D(_MainTex, i.uv);
                
                // Apply vertex color
                fixed4 col = texColor * i.color * _Color;
                
                // Dashed effect
                if (_UseDashed > 0.5)
                {
                    float dashPattern = fmod(i.uv.x, _DashLength + _DashGap);
                    if (dashPattern > _DashLength)
                    {
                        discard;
                    }
                }
                
                // Edge softness (gradient from center to edge)
                float edgeFade = 1.0 - abs(i.uv.y - 0.5) * 2.0;
                edgeFade = smoothstep(0.0, _Softness, edgeFade);
                col.a *= edgeFade;
                
                // Glow effect
                if (_UseGlow > 0.5)
                {
                    float glow = pow(edgeFade, 0.5) * _GlowIntensity;
                    col.rgb += _GlowColor.rgb * glow;
                    col.a += glow * 0.3;
                }
                
                // Sparkles effect
                if (_UseSparkles > 0.5)
                {
                    float2 sparkleUV = i.uv * 20.0;
                    sparkleUV.x += _Time.y * _SparkleSpeed;
                    
                    float sparkle = noise(sparkleUV);
                    sparkle = pow(sparkle, 8.0);
                    
                    if (sparkle > 0.95)
                    {
                        col.rgb += float3(1, 1, 1) * (sparkle - 0.95) * 20.0;
                    }
                }
                
                // Apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                
                return col;
            }
            ENDCG
        }
    }
    
    Fallback "Sprites/Default"
}
