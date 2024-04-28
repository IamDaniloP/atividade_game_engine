using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProjetoFinalGameEngine
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //Variáveis comuns
        Texture2D backgroundSprite;
        Texture2D crosshairSprite;
        SpriteFont gameFont;


        //Início variáveis p/ Menu do Jogo
        bool menu = true; // Deve verificar se está no Menu

        Texture2D playButton;
        Rectangle botaoIniciar;
        Rectangle botaoCreditos;
        float timerMenu = 0;

        Texture2D whitePixel;

        private void CreateWhitePixel()
        {
            whitePixel = new Texture2D(GraphicsDevice, 1, 1);
            whitePixel.SetData(new[] { Color.White });
        }

        //Fim variáveis p/ Menu do Jogo

        // Início variáveis p/ Lógica do Jogo
        Texture2D targetSprite;
        Texture2D winSprite;
        Texture2D gameOverSprite;


        private Texture2D _lifeAnimation;
        private Rectangle[] _frames;


        Vector2 targetPosition = new Vector2(300, 300);
        const int targetRadius = 45;

        MouseState mState;
        bool mReleased = true;
        bool playing = false;
        bool end = false;
        int score = 0;
        int life = 3;
        double timer = 10;
        // Fim variáveis p/ Lógica do Jogo

        //Incio variáveis p/ Crédito do Jogo
        List<string> _nomesCriadores;
        Vector2 _posicaoNomes;

        bool creditos = false;
        private float _scrollOffset = 0f;
        private const float ScrollSpeed = 69f;
        //Fim variáveis p/ Crédito do Jogo

        private Stopwatch _timerStopwatch = new Stopwatch();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1180;
            _graphics.PreferredBackBufferHeight = 700;
            _graphics.ApplyChanges();

            _frames = new Rectangle[4]
            {
                new Rectangle(0, 0, 150, 50), new Rectangle(150, 0, 150, 50), new Rectangle(300, 0, 150, 50), new Rectangle(450, 0, 150, 50)
            };

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //Variáveis globais
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            backgroundSprite = Content.Load<Texture2D>("sky");
            crosshairSprite = Content.Load<Texture2D>("crosshairs");
            gameFont = Content.Load<SpriteFont>("galleryFont");

            //Variáveis p/ Menu
            botaoIniciar = new Rectangle(550, 380, 160, 80);
            playButton = Content.Load<Texture2D>("playmadeira");
            botaoCreditos = new Rectangle(520, 550, 200, 200);
            CreateWhitePixel();

            //Variáveis p/ Lógica do Jogo 
            targetSprite = Content.Load<Texture2D>("target");
            _lifeAnimation = Content.Load<Texture2D>("life_animation");
            winSprite = Content.Load<Texture2D>("vitoria");
            gameOverSprite = Content.Load<Texture2D>("game_over");

            //Variáveis p/ Crédito do Jogo
            _nomesCriadores = new List<string>
            {
                "Menu",
                "Guilherme Freitas - 0156483",
                "Maria Luiza - 01601881",
                " ",
                "Desenvolvimento do Jogo",
                "Jose Willian Guilherme - 01553544",
                "Pedro Ricardo - 01565486",
                " ",
                "Creditos",
                "Danilo Pereira - 01561356",
                "Geraldo Junior - 01574443",
                " ",
                "Finalizacao",
                "Danilo Pereira - 01561356",
                "Geraldo Junior - 01574443",
            };

            _posicaoNomes = new Vector2(50, 700);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            mState = Mouse.GetState();

            if (botaoIniciar.Contains(mState.Position))
            {
                if (mState.LeftButton == ButtonState.Pressed && mReleased == true)
                {
                    menu = false;
                    playing = true;
                    mReleased = false;
                    iniciaJogo(gameTime);
                }
            }

            if (playing)
            {
                iniciaJogo(gameTime);
            }

            if (botaoCreditos.Contains(mState.Position))
            {
                if (mState.LeftButton == ButtonState.Pressed && mReleased == true)
                {
                    mReleased = false;
                    iniciaCreditos();
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            _spriteBatch.Begin();
            _spriteBatch.Draw(backgroundSprite, new Vector2(0, 0), Color.White);

            if (menu)
            {
                iniciaMenu();
            } else if (!menu && playing)
            {
                desenhaJogo();
            }

            if (timer < 0 || life == 0 && end)
            {
                desenhaDerrota(gameTime);
            }

            if (score == 10 && end)
            {
                desenhaVitoria(gameTime);
            }

            if (creditos && !menu & !playing)
            {
                desenhaCreditos(gameTime);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void iniciaMenu()
        {
            _spriteBatch.Draw(targetSprite, new Vector2(_graphics.PreferredBackBufferWidth / 2 + 280, _graphics.PreferredBackBufferHeight / 2 - 300), Color.White);
            _spriteBatch.Draw(targetSprite, new Vector2(_graphics.PreferredBackBufferWidth / 2 - 360, _graphics.PreferredBackBufferHeight / 2 - 300), Color.White);
            _spriteBatch.DrawString(gameFont, "Mata Mosquito", new Vector2(_graphics.PreferredBackBufferWidth / 2 - 205, _graphics.PreferredBackBufferHeight / 2 - 280), Color.White, 0f, Vector2.Zero, 80 / gameFont.MeasureString("Creditos").Y, SpriteEffects.None, 0f);
            _spriteBatch.DrawString(gameFont, "Creditos", new Vector2(520, 550), Color.White, 0f, Vector2.Zero, 50 / gameFont.MeasureString("Creditos").Y, SpriteEffects.None, 0f);

            _spriteBatch.Draw(playButton, new Vector2(_graphics.PreferredBackBufferWidth / 2 - 70, _graphics.PreferredBackBufferHeight / 2), Color.White);
            _spriteBatch.Draw(crosshairSprite, new Vector2(mState.X - 25, mState.Y - 25), Color.White);
        }

        private void iniciaJogo(GameTime gameTime)
        {
            if (playing)
            {
                if (mState.LeftButton == ButtonState.Pressed && mReleased == true)
                {
                    float mouseTargetDist = Vector2.Distance(targetPosition, mState.Position.ToVector2());
                    if (mouseTargetDist < targetRadius)
                    {
                        score++;

                        Random rand = new Random();

                        targetPosition.X = rand.Next(targetRadius, _graphics.PreferredBackBufferWidth - targetRadius);
                        targetPosition.Y = rand.Next(targetRadius, _graphics.PreferredBackBufferHeight - targetRadius);
                    }
                    else
                    {
                        life--;
                    }

                    mReleased = false;
                }

                if (mState.LeftButton == ButtonState.Released)
                {
                    mReleased = true;
                }

                if (timer > 0)
                {
                    timer -= gameTime.ElapsedGameTime.TotalSeconds;
                }

                //win
                if (score == 10)
                {
                    playing = false;
                    end = true;
                }

                //lose
                if (timer < 0 || life == 0)
                {
                    playing = false;
                    end = true;
                }
            }
        }

        private void desenhaJogo()
        {
            if (playing)
            {
                _spriteBatch.DrawString(gameFont, "Score: " + score.ToString() + "/ 10", new Vector2(3, 3), Color.White);
                _spriteBatch.DrawString(gameFont, "Time: " + Math.Ceiling(timer).ToString(), new Vector2(3, 40), Color.White);

                if (timer > 0)
                {
                    _spriteBatch.Draw(targetSprite, new Vector2(targetPosition.X - targetRadius, targetPosition.Y - targetRadius), Color.White);
                }

                _spriteBatch.Draw(crosshairSprite, new Vector2(mState.X - 25, mState.Y - 25), Color.White);

                if (life > 0)
                {
                    _spriteBatch.Draw(_lifeAnimation, new Rectangle(1000, 20, 150, 50), _frames[life], Color.White);
                }
            }
        }

        private void desenhaDerrota(GameTime gameTime)
        {
            _spriteBatch.Draw(backgroundSprite, new Vector2(0, 0), Color.White);
            _spriteBatch.Draw(gameOverSprite, new Vector2(350, 80), Color.White);
            _spriteBatch.Draw(crosshairSprite, new Vector2(mState.X - 25, mState.Y - 25), Color.White);

            if (!_timerStopwatch.IsRunning)
            {
                _timerStopwatch.Restart();
            }
            if (_timerStopwatch.Elapsed.TotalSeconds > 2)
            {
                ReiniciarJogo();
                _timerStopwatch.Stop();
            }
        }

        private void desenhaVitoria(GameTime gameTime)
        {
            _spriteBatch.Draw(backgroundSprite, new Vector2(0, 0), Color.White);
            _spriteBatch.Draw(winSprite, new Vector2(400, 80), Color.White);
            _spriteBatch.Draw(crosshairSprite, new Vector2(mState.X - 25, mState.Y - 25), Color.White);

            if (!_timerStopwatch.IsRunning)
            {
                _timerStopwatch.Restart();
            }

            if (_timerStopwatch.Elapsed.TotalSeconds > 2)
            {
                ReiniciarJogo();
                _timerStopwatch.Stop();
            }
        }

        private void resetaJogo()
        {
            mReleased = true;
            playing = false;
            end = false;
            score = 0;
            life = 3;
            timer = 10;
        }

        private void ReiniciarJogo()
        {
            resetaJogo(); 
            menu = true;
            creditos = false;
        }

        private void iniciaCreditos()
        {
            creditos = true;
            playing = false;
            menu = false;
        }

        private void desenhaCreditos(GameTime gameTime)
        {
            _scrollOffset += ScrollSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            float totalNamesHeight = _nomesCriadores.Count * gameFont.LineSpacing;
            float yPos = _graphics.PreferredBackBufferHeight;

            if (_scrollOffset > totalNamesHeight)
            {
                _scrollOffset -= totalNamesHeight;
            }

            yPos -= _scrollOffset;
            yPos -= totalNamesHeight;

            foreach (string nome in _nomesCriadores)
            {
                if (yPos + totalNamesHeight > 0 && yPos < _graphics.PreferredBackBufferHeight)
                {
                    float nameWidth = gameFont.MeasureString(nome).X;
                    _spriteBatch.DrawString(gameFont, nome, new Vector2((_graphics.PreferredBackBufferWidth - nameWidth) / 2, yPos), Color.White);
                }

                yPos += gameFont.LineSpacing;
            }

            if (!_timerStopwatch.IsRunning)
            {
                _timerStopwatch.Restart();
            }
            if (_timerStopwatch.Elapsed.TotalSeconds > 8)
            {
                ReiniciarJogo();
                _timerStopwatch.Stop();
            }
        }



    }
}
