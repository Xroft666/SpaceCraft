using UnityEngine;
using System.Collections;

using pacman;
using pacman.controllers;
using pacman.controllers.examples;
using pacman.game;

using java.util;

public class Executor : MonoBehaviour 
{
	Game game = new Game(0L);

	Controller pacManController = new StarterPacMan();
	Controller ghostController = new StarterGhosts();

	IEnumerator Start()
	{
		float delay = Time.deltaTime;

		while (!game.gameOver())
		{
			game.advanceGame((Constants.MOVE)pacManController.getMove(game.copy(), -1L), (EnumMap)ghostController.getMove(game.copy(), -1L));
			yield return new WaitForSeconds(delay);
		}
	}

//	public static void saveToFile(string data, string name, bool append)
//	{
//			FileOutputStream fileOutputStream = new FileOutputStream(name, append);
//			PrintWriter printWriter = new PrintWriter(fileOutputStream);
//			printWriter.println(data);
//			printWriter.flush();
//			fileOutputStream.close();
//
//	}

//	public virtual void replayGame(string fileName, bool visual)
//	{
//		ArrayList arrayList = Executor.loadReplay(fileName);
//		Game game = new Game(0L);
//		GameView gameView = null;
//		if (visual)
//		{
//			gameView = new GameView(game).showGame();
//		}
//		int i = 0;
//		while (i < arrayList.size())
//		{
//			game.setGameState((string)arrayList.get(i));
//			InterruptedException ex;
//			try
//			{
//				Thread.sleep((long)((ulong)40));
//			}
//			catch (InterruptedException arg_52_0)
//			{
//				ex = ByteCodeHelper.MapException<InterruptedException>(arg_52_0, ByteCodeHelper.MapFlags.NoRemapping);
//				goto IL_5D;
//			}
//		IL_6C:
//				if (visual)
//			{
//				gameView.repaint();
//			}
//			i++;
//			continue;
//			goto IL_6C;
//		IL_5D:
//				InterruptedException this2 = ex;
//			Throwable.instancehelper_printStackTrace(this2);
//			goto IL_6C;
//		}
//	}
//
//	public virtual void runExperiment(Controller pacManController, Controller ghostController, int trials)
//	{
//		double num = (double)0f;
//		Random random = new Random(0L);
//		for (int i = 0; i < trials; i++)
//		{
//			Game.__<clinit>();
//			Game game = new Game(random.nextLong());
//			while (!game.gameOver())
//			{
//				game.advanceGame((Constants.MOVE)pacManController.getMove(game.copy(), System.currentTimeMillis() + (long)((ulong)40)), (EnumMap)ghostController.getMove(game.copy(), System.currentTimeMillis() + (long)((ulong)40)));
//			}
//			num += (double)game.getScore();
//			System.@out.println(new StringBuilder().append(i).append("\t").append(game.getScore()).toString());
//		}
//		System.@out.println(num / (double)trials);
//	}
//
//
//	public virtual void runGame(Controller pacManController, Controller ghostController, bool visual, int delay)
//	{
//		Game game = new Game(0L);
//		GameView gameView = null;
//		if (visual)
//		{
//			gameView = new GameView(game).showGame();
//		}
//		while (!game.gameOver())
//		{
//			game.advanceGame((Constants.MOVE)pacManController.getMove(game.copy(), -1L), (EnumMap)ghostController.getMove(game.copy(), -1L));
//			try
//			{
//				Thread.sleep((long)delay);
//			}
//			catch (Exception arg_5F_0)
//			{
//				if (ByteCodeHelper.MapException<Exception>(arg_5F_0, ByteCodeHelper.MapFlags.Unused) == null)
//				{
//					throw;
//				}
//			}
//		IL_72:
//				if (visual)
//			{
//				gameView.repaint();
//				continue;
//			}
//			continue;
//			goto IL_72;
//		}
//	}
//
//	public virtual void runGameTimed(Controller pacManController, Controller ghostController, bool visual)
//	{
//		Game game = new Game(0L);
//		GameView gameView = null;
//		if (visual)
//		{
//			gameView = new GameView(game).showGame();
//		}
//		if (pacManController is HumanController)
//		{
//			gameView.getFrame().addKeyListener(((HumanController)pacManController).getKeyboardInput());
//		}
//		new Thread(pacManController).start();
//		new Thread(ghostController).start();
//		while (!game.gameOver())
//		{
//			pacManController.update(game.copy(), System.currentTimeMillis() + (long)((ulong)40));
//			ghostController.update(game.copy(), System.currentTimeMillis() + (long)((ulong)40));
//			InterruptedException ex;
//			try
//			{
//				Thread.sleep((long)((ulong)40));
//			}
//			catch (InterruptedException arg_91_0)
//			{
//				ex = ByteCodeHelper.MapException<InterruptedException>(arg_91_0, ByteCodeHelper.MapFlags.NoRemapping);
//				goto IL_9B;
//			}
//		IL_A9:
//				game.advanceGame((Constants.MOVE)pacManController.getMove(), (EnumMap)ghostController.getMove());
//			if (visual)
//			{
//				gameView.repaint();
//				continue;
//			}
//			continue;
//			goto IL_A9;
//		IL_9B:
//				InterruptedException this2 = ex;
//			Throwable.instancehelper_printStackTrace(this2);
//			goto IL_A9;
//		}
//		pacManController.terminate();
//		ghostController.terminate();
//	}
//
//	public virtual void runGameTimedRecorded(Controller pacManController, Controller ghostController, bool visual, string fileName)
//	{
//		StringBuilder stringBuilder = new StringBuilder();
//		Game game = new Game(0L);
//		GameView gameView = null;
//		if (visual)
//		{
//			gameView = new GameView(game).showGame();
//			if (pacManController is HumanController)
//			{
//				gameView.getFrame().addKeyListener(((HumanController)pacManController).getKeyboardInput());
//			}
//		}
//		new Thread(pacManController).start();
//		new Thread(ghostController).start();
//		while (!game.gameOver())
//		{
//			pacManController.update(game.copy(), System.currentTimeMillis() + (long)((ulong)40));
//			ghostController.update(game.copy(), System.currentTimeMillis() + (long)((ulong)40));
//			InterruptedException ex;
//			try
//			{
//				Thread.sleep((long)((ulong)40));
//			}
//			catch (InterruptedException arg_97_0)
//			{
//				ex = ByteCodeHelper.MapException<InterruptedException>(arg_97_0, ByteCodeHelper.MapFlags.NoRemapping);
//				goto IL_A2;
//			}
//		IL_B1:
//				game.advanceGame((Constants.MOVE)pacManController.getMove(), (EnumMap)ghostController.getMove());
//			if (visual)
//			{
//				gameView.repaint();
//			}
//			stringBuilder.append(new StringBuilder().append(game.getGameState()).append("\n").toString());
//			continue;
//			goto IL_B1;
//		IL_A2:
//				InterruptedException this2 = ex;
//			Throwable.instancehelper_printStackTrace(this2);
//			goto IL_B1;
//		}
//		pacManController.terminate();
//		ghostController.terminate();
//		Executor.saveToFile(stringBuilder.toString(), fileName, false);
//	}
//
//	public virtual void runGameTimedSpeedOptimised(Controller pacManController, Controller ghostController, bool fixedTime, bool visual)
//	{
//		Game game = new Game(0L);
//		GameView gameView = null;
//		if (visual)
//		{
//			gameView = new GameView(game).showGame();
//		}
//		if (pacManController is HumanController)
//		{
//			gameView.getFrame().addKeyListener(((HumanController)pacManController).getKeyboardInput());
//		}
//		new Thread(pacManController).start();
//		new Thread(ghostController).start();
//		while (!game.gameOver())
//		{
//			pacManController.update(game.copy(), System.currentTimeMillis() + (long)((ulong)40));
//			ghostController.update(game.copy(), System.currentTimeMillis() + (long)((ulong)40));
//			InterruptedException ex;
//			try
//			{
//				int num = 40;
//				for (int i = 0; i < 40; i++)
//				{
//					Thread.sleep(1L);
//					if (pacManController.hasComputed() && ghostController.hasComputed())
//					{
//						num = i;
//						break;
//					}
//				}
//				if (fixedTime)
//				{
//					Thread.sleep((long)((40 - num) * 1));
//				}
//				game.advanceGame((Constants.MOVE)pacManController.getMove(), (EnumMap)ghostController.getMove());
//			}
//			catch (InterruptedException arg_EA_0)
//			{
//				ex = ByteCodeHelper.MapException<InterruptedException>(arg_EA_0, ByteCodeHelper.MapFlags.NoRemapping);
//				goto IL_F5;
//			}
//		IL_104:
//				if (visual)
//			{
//				gameView.repaint();
//				continue;
//			}
//			continue;
//			goto IL_104;
//		IL_F5:
//				InterruptedException this2 = ex;
//			Throwable.instancehelper_printStackTrace(this2);
//			goto IL_104;
//		}
//		pacManController.terminate();
//		ghostController.terminate();
//	}

}
