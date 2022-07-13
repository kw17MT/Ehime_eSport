using CriWare;
using UnityEngine;

namespace nsSound
{
    //サウンドソース。
    public class SoundSource : MonoBehaviour
    {
        //サウンドマネージャーのインスタンスを入れるための変数
        private SoundManager m_soundManager;

        //キューシートがロード中かどうか true:ロード中 false:ロード済み
        private bool m_cueSheetsAreLoading = true;
        //このサウンドソース固有のボリューム
        private float m_sourceVolume = 1.0f;
        //サウンドの分類によるボリューム
        private float m_soundTypeVolume = 1.0f;

        //このサウンドソースが再生するサウンドの分類
        private EnSoundTypes m_enSoundType = EnSoundTypes.enNone;

        //再生停止のフェードアウト中かどうか
        private bool m_isDoingFadeOut = false;

        //CRIADX2関連
        //キュー情報の配列
        private CriAtomEx.CueInfo[] m_cueInfoList = null;
        //サウンドを再生するためのプレイヤー(再生機)
        private CriAtomExPlayer m_atomExPlayer = null;
        //ACB、AWBファイルのキュー情報を管理するクラス。
        private CriAtomExAcb m_atomExAcb = null;
        //3D関連
        // 3Dサウンドのリスナー(聴く側)
        private CriAtomEx3dListener m_atomEx3Dlistener = null;
        // 3Dサウンドのソース(鳴る側)
        private CriAtomEx3dSource m_atomEx3DSource = null;

        // Start is called before the first frame update
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            //プレイヤーを破棄
            if (m_atomExPlayer != null)
            {
                m_atomExPlayer.Dispose();
            }
            //3Dリスナーの破棄
            if (m_atomEx3Dlistener != null)
            {
                m_atomEx3Dlistener.Dispose();
            }
            //3Dサウンドソースの破棄
            if (m_atomEx3DSource != null)
            {
                m_atomEx3DSource.Dispose();
            }

        }
        // Update is called once per frame
        void Update()
        {
            Debug.Log(m_atomExPlayer.GetStatus());

            //再生し終わったら、消す。
            if (m_atomExPlayer.GetStatus() == CriAtomExPlayer.Status.PlayEnd)
            {
                Destroy(this.gameObject.transform.root.gameObject);
            }

            //フェードアウトしているならば、
            if (m_isDoingFadeOut == true)
            {
                DoFadeOut();
            }
        }

        //初期化。ただし、キューシートがロード済みでない場合は初期化が行えないため、
        //あらゆる関数呼び出しの始めにこの関数を呼び出してロード済みかどうかを調べ、
        //ロード済みならば一度だけ初期化処理を行う。
        private bool Init()
        {
            //キューシートがロード済みかどうか
            if (m_cueSheetsAreLoading == false)
            {
                // ロード済みだから初期化処理をしない
                Debug.Log("初期化済みです。");
                return true;
            }
            else
            {
                //キューシートのロードが完了していたら、
                if (CriAtom.CueSheetsAreLoading == false)
                {
                    //ロード中かどうかをfalseに。(ロード済みに)
                    m_cueSheetsAreLoading = CriAtom.CueSheetsAreLoading;
                }
                //まだロード中なら、何もしない。
                else
                {
                    Debug.Log("まだキューシートのロード中です。");
                    return false;
                }
            }

            Debug.Log("ロードが完了したので初期化します。");

            //サウンドマネージャーのインスタンスを取得。
            m_soundManager = SoundManager.Instance;

            /* AtomExPlayerの生成 */
            m_atomExPlayer = new CriAtomExPlayer();

            //ここでこのサウンドの分類に合わせたキューシートと音量を設定するようにする。
            switch (m_enSoundType)
            {
                case EnSoundTypes.enSE:
                    m_soundTypeVolume = m_soundManager.GetSEVolume();
                    m_atomExAcb = CriAtom.GetAcb("CueSheet_SE");
                    break;
                case EnSoundTypes.enBGM:
                    m_soundTypeVolume = m_soundManager.GetSEVolume();
                    m_atomExAcb = CriAtom.GetAcb("CueSheet_BGM");
                    break;
                case EnSoundTypes.enNarration:
                    m_soundTypeVolume = m_soundManager.GetSEVolume();
                    m_atomExAcb = CriAtom.GetAcb("test03");
                    break;
                case EnSoundTypes.enCharaVoice:
                    m_soundTypeVolume = m_soundManager.GetSEVolume();
                    m_atomExAcb = CriAtom.GetAcb("test03");
                    break;
                default:
                    break;
            }
            //キューの情報を取得する。
            m_cueInfoList = m_atomExAcb.GetCueInfoList();

            //フェーダをアタッチする。これによって、フェードの処理が行えるようになる。
            m_atomExPlayer.AttachFader();

            return true;
        }

        //サウンドのタイプを設定する。(必須)
        //サウンドソースを生成したら、まず初めにこの関数を呼び出してください。
        public void SetSoundType(EnSoundTypes enSoundType)
        {
            m_enSoundType = enSoundType;
        }

        //サウンドを再生開始する。 
        //引数:soundNameには、サウンドの名前を渡します。
        //定数を用意するので、nsSound.○○○Names.m_k○○○ のような形で利用してください。
        //SetFadeInTime関数でフェードインの時間を設定していた場合、フェードインを行います。
        //その後、音声再生中にもう一度PlayStart関数を呼び出すと、クロスフェードが行えます。
        public void PlayStart(string soundName)
        {
            if (Init() != true)
            {
                return;
            }
            //キューの設定
            m_atomExPlayer.SetCue(m_atomExAcb, soundName);
            //再生開始
            m_atomExPlayer.Start();
        }

        //再生開始するまでの時間を設定します。引数:msに渡す値は1000で1秒
        //この関数に値を設定した後PlayStart関数が呼ばれると、
        //設定した時間が経ってから音声が再生され始めます。
        public void SetPlayStartOffset(int ms)
        {
            m_atomExPlayer.SetFadeInStartOffset(ms);
        }

        //このサウンドソースの音量を設定(他のサウンドとは関係ない固有のパラメータ)
        public void SetSourceVolume(float volume)
        {
            if (Init() != true)
            {
                return;
            }
            m_sourceVolume = volume;
            //ソースの音量と、このソースの分類の音量と、全体の音量で、最終の音量を設定する。
            m_atomExPlayer.SetVolume(m_sourceVolume * m_soundTypeVolume * m_soundManager.GetAllVolume());
            m_atomExPlayer.UpdateAll();
        }

        //サウンドを停止する。このサウンドソースは消えます。
        public void Stop()
        {
            if (Init() != true)
            {
                return;
            }

            m_atomExPlayer.Stop();
            Destroy(this.gameObject.transform.root.gameObject);
        }

        //フェードアウトします。フェードアウトが終わると、このサウンドソースは消えます。
        //ただしフェードアウトやフェードイン中にこの関数が呼ばれた場合、ぶつ切りになります。
        //SetFadeOutTime関数で設定した時間は適応されます。
        public void FadeOutStart()
        {
            if (Init() != true)
            {
                return;
            }

            //フェードアウト中に設定。
            m_isDoingFadeOut = true;
            m_atomExPlayer.Stop(false);
        }

        //フェードアウト中の処理
        private void DoFadeOut()
        {
            //フェードアウトが終わったら、消す。
            if (m_atomExPlayer.GetStatus() == CriAtomExPlayer.Status.Stop)
            {
                Destroy(this.gameObject.transform.root.gameObject);
            }
        }

        //ポーズ。音声を一時停止します。
        //Resume関数を呼ぶことでポーズが解除され、一時停止したところから再生されます。
        public void Pause()
        {
            if (Init() != true)
            {
                return;
            }
            m_atomExPlayer.Pause();
        }

        //ポーズ状態を解除して、音声を一時停止されていたところから再生します。
        public void Resume()
        {
            if (Init() != true)
            {
                return;
            }
            m_atomExPlayer.Resume(CriAtomEx.ResumeMode.AllPlayback);
        }

        //ループ再生するかどうかを設定します。true:ループする。 false:ループしない。
        public void SetLoop(bool sw)
        {
            if (Init() != true)
            {
                return;
            }
            m_atomExPlayer.Loop(sw);
        }

        //ピッチを設定します。引数:pitchは、0.0fが通常。100.0fで半音上がり、-100.0fで半音下がります。
        public void SetPitch(float pitch)
        {
            if (Init() != true)
            {
                return;
            }
            m_atomExPlayer.SetPitch(pitch);
            m_atomExPlayer.UpdateAll();
        }

        //シーケンス再生レシオの設定らしいです。
        public void SetPlaybackRatio(float ratio)
        {
            if (Init() != true)
            {
                return;
            }
            m_atomExPlayer.SetPlaybackRatio(ratio);
            m_atomExPlayer.UpdateAll();
        }

        ////////////////////////////////////////////////////////////////////////

        //フェードイン・フェードアウト関係。
        //これらとSetPlayStartOffset関数を利用することで、クロスフェードを行えます。

        //フェードインの時間を設定します。引数:msに渡す値は1000で1秒
        //フェードインの時間を設定すると、PlayStart関数が呼び出された時、自動でフェードインが行われます。
        public void SetFadeInTime(int ms)
        {
            m_atomExPlayer.SetFadeInTime(ms);
        }
        //フェードアウトの時間を設定します。引数:msに渡す値は1000で1秒
        public void SetFadeOutTime(int ms)
        {
            m_atomExPlayer.SetFadeOutTime(ms);
        }

        /////////////////////////////////////////////////////////////

        //3Dサウンド関係。Be3DSound()を呼び出すと、3Dサウンドになります。

        //3Dサウンドにする。
        public void Be3DSound()
        {
            if (Init() != true)
            {
                return;
            }
            //3Dサウンドのリスナの作成
            m_atomEx3Dlistener = new CriAtomEx3dListener();
            //3Dサウンドのソースの作成
            m_atomEx3DSource = new CriAtomEx3dSource();
            // ソース、リスナをプレーヤに設定
            m_atomExPlayer.Set3dListener(m_atomEx3Dlistener);
            m_atomExPlayer.Set3dSource(m_atomEx3DSource);
        }

        //3Dサウンド用。リスナーの位置を設定する。基本的にゲームカメラの位置を設定する。
        public void Set3DListenerPos(Vector3 pos)
        {
            if (Init() != true)
            {
                return;
            }
            m_atomEx3Dlistener.SetPosition(pos.x, pos.y, pos.z);
            m_atomEx3Dlistener.Update();
        }
        public void Set3DListenerPos(float x, float y, float z)
        {
            if (Init() != true)
            {
                return;
            }
            m_atomEx3Dlistener.SetPosition(x, y, z);
            m_atomEx3Dlistener.Update();
        }
        //3Dサウンド用。サウンドが鳴る位置を設定する。
        public void Set3DSourcePos(Vector3 pos)
        {
            if (Init() != true)
            {
                return;
            }
            m_atomEx3DSource.SetPosition(pos.x, pos.y, pos.z);
            m_atomEx3DSource.Update();
        }
        public void Set3DSourcePos(float x, float y, float z)
        {
            if (Init() != true)
            {
                return;
            }
            m_atomEx3DSource.SetPosition(x, y, z);
            m_atomEx3DSource.Update();
        }

        //3Dサウンドが聞こえる最小距離と最大距離を設定する。
        //最小距離は、それ以上音の大きさが大きくならない距離
        //最大距離は、音が聞こえなくなる距離。
        public void Set3DMinMaxDistance(float minDis, float maxDis)
        {
            if (Init() != true)
            {
                return;
            }
            m_atomEx3DSource.SetMinMaxDistance(minDis, maxDis);
            m_atomEx3DSource.Update();
        }

    }
}