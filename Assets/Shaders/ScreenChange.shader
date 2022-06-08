//シェーダーのPostEffectの欄にScreenChangeを追加する
Shader "PostEffect/ScreenChange"
{
	Properties
	{
		//Graphics.Blit()によってレンダリング済の画像が送られてくる
		//名前は_MainTex固定
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Pass{
			CGPROGRAM

			//頂点シェーダーのメイン関数はテンプレートの「vert_img」を使う
			//フラグメントシェーダーのメイン関数を「frag」と定義
#include "UnityCG.cginc"
#pragma vertex vert_img
#pragma fragment frag

			//Propertiesで用意した変数をここで受け取る
			sampler2D _MainTex;

			//スクリプトから値を受け取る
			float _FadeCount;

			//メイン関数（フラグメントシェーダー）
			fixed4 frag(v2f_img i) : SV_TARGET{
				fixed4 c;

			float fadeSize;
			float visible;

			////横切れ込みフェード
			//fadeSize = 32.0f;	//フェードの大きさ
			//visible = 1.0f - floor(frac(i.uv.x / (fadeSize / _ScreenParams.x)) + _FadeCount);

			fadeSize = distance(float2(0.0, 0.0), float2(0.5, 0.5) * _ScreenParams.xy);
			//中央からの距離
			float dist = distance(float2(0.5, 0.5) * _ScreenParams.xy, i.uv * _ScreenParams.xy);
			visible = clamp((1.0f - _FadeCount) * fadeSize - dist, 0.0, 1.0);

			c = fixed4(tex2D(_MainTex, i.uv).rgb * visible, 1.0f);

				return c;
			}

			ENDCG
		}
	}
}