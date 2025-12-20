Shader "Custom/Title"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Base Color", Color) = (1,1,1,1)
        _GradientTop ("Gradient Top", Color) = (1,0.8,0.2,1)
        _GradientBottom ("Gradient Bottom", Color) = (1,0.4,0.1,1)
        _GlowIntensity ("Glow Intensity", Range(0,5)) = 2.5
        _GlowSpeed ("Glow Speed", Range(0,8)) = 3.0
        _OutlineColor ("Outline Color", Color) = (0.2,0.1,0.8,1)
        _WaveAmplitude ("Wave Amplitude", Range(0,0.1)) = 0.02
        _WaveFrequency ("Wave Frequency", Range(0,20)) = 10.0
        _WaveSpeed ("Wave Speed", Range(0,10)) = 3.0
        _BloomIntensity ("Bloom Intensity", Range(0,3)) = 1.8
        _PulseSpeed ("Pulse Speed", Range(0,10)) = 4.0
        _SparkleIntensity ("Sparkle Intensity", Range(0,2)) = 1.2
        _RimPower ("Rim Power", Range(1,10)) = 3.0
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
        }
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 worldPos : TEXCOORD1;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _GradientTop;
            fixed4 _GradientBottom;
            float _GlowIntensity;
            float _GlowSpeed;
            fixed4 _OutlineColor;
            float _WaveAmplitude;
            float _WaveFrequency;
            float _WaveSpeed;
            float _BloomIntensity;
            float _PulseSpeed;
            float _SparkleIntensity;
            float _RimPower;
            
            v2f vert (appdata v)
            {
                v2f o;
                
                float wave = sin(v.uv.x * _WaveFrequency + _Time.y * _WaveSpeed) * _WaveAmplitude;
                v.vertex.y += wave;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                
                // グラデーション
                float gradientFactor = 1.0 - i.uv.y;
                fixed4 gradientColor = lerp(_GradientBottom, _GradientTop, gradientFactor);
                
                // メインのグロー効果（強化）
                float glow = sin(_Time.y * _GlowSpeed) * 0.5 + 0.5;
                glow = pow(glow, 1.5) * _GlowIntensity;
                
                // パルス効果（二重のリズム）
                float pulse1 = sin(_Time.y * _PulseSpeed) * 0.5 + 0.5;
                float pulse2 = sin(_Time.y * _PulseSpeed * 0.7) * 0.3 + 0.7;
                float combinedPulse = pulse1 * pulse2;
                
                // 強化されたスパークル（複数レイヤー）
                float sparkle1 = sin(i.uv.x * 15.0 + _Time.y * 3.0) * 0.5 + 0.5;
                float sparkle2 = sin(i.uv.x * 25.0 + i.uv.y * 20.0 + _Time.y * 5.0) * 0.3 + 0.7;
                float sparkle3 = sin(i.uv.x * 8.0 - _Time.y * 2.0) * 0.4 + 0.6;
                float totalSparkle = (sparkle1 + sparkle2 + sparkle3) * _SparkleIntensity;
                
                // リム（輪郭）ライティング
                float rim = 1.0 - abs(dot(normalize(float3(0, 0, 1)), float3(i.uv.x - 0.5, i.uv.y - 0.5, 0)));
                rim = pow(rim, _RimPower);
                
                // ランダムなきらめき点
                float randomSparkle = frac(sin(dot(i.uv.xy, float2(12.9898, 78.233))) * 43758.5453);
                randomSparkle = step(0.98, randomSparkle) * sin(_Time.y * 10.0) * 0.5 + 0.5;
                
                // 色の合成
                col *= gradientColor;
                
                // グロー効果を強化
                col.rgb += glow * 0.6 * _GradientTop.rgb;
                col.rgb += combinedPulse * 0.4 * _GradientTop.rgb;
                
                // スパークル効果
                col.rgb += totalSparkle * 0.3 * _OutlineColor.rgb;
                col.rgb += randomSparkle * 0.8 * float3(1,1,0.8);
                
                // リムライト
                col.rgb += rim * 0.5 * _OutlineColor.rgb;
                
                // ブルーム効果のシミュレーション
                float bloom = (glow + combinedPulse + totalSparkle * 0.5) * _BloomIntensity;
                col.rgb += bloom * 0.3 * _GradientTop.rgb;
                
                // 全体の輝度を上げる
                col.rgb *= 1.3;
                
                return col;
            }
            ENDCG
        }
    }
    FallBack "Sprites/Default"
}
