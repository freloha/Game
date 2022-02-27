using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCat
{
    class Game
    {
        private int remain;//현재 남은 핀
        private int frame;//현재 프레임
        private int endFrame;//프레임 끝, 10
        private int throwBall;//현재 투구횟수
        private int[][] score;//점수 저장, 스코어가 -1인 경우 아직 해당 프레임에 투구하지 않았음을 의미
        private int[] total;

        public Game()
        {
            remain = 10;
            frame = 0;
            endFrame = 10;
            throwBall = 0;

            score = new int[endFrame][];
            for (int i = 0; i < endFrame - 1; i++)
                score[i] = new int[2] { -1, -1};
            score[9] = new int[3] { -1, -1, -1};

            total = new int[endFrame];
        }

        public void KnockedDownPins(int pin)
        {
            Console.WriteLine("Frame : " + (frame+1) + ", 공 던진 횟수 : "+ (throwBall+1) + "쓰러트린 핀의 수 : " + pin + ", 남은 핀의 수 : " + remain);
            if(remain < pin || pin < 0 || pin > 10 || frame > 9 || throwBall > 2)//에러메세지 조건
            {
                Console.WriteLine("**************************\n올바르지 않은 값을 입력받았습니다.\n**************************\n");
            }
            else if(frame == 9)//10 프레임
            {
                if(throwBall == 0 && pin == 10)//스트라이크
                {
                    score[frame][throwBall++] = pin;
                }
                else if(throwBall == 1 && remain == pin)//스페어
                {
                    score[frame][throwBall++] = pin;
                    remain = 10;
                }
                else
                {
                    if (throwBall == 1 && remain-pin != 0)//끝
                    {
                        if(score[frame][throwBall-1] == 10)
                            score[frame][throwBall++] = pin;
                        else
                            score[frame++][throwBall] = pin;//3번째 투구, 3번쨰 투구 시에는 프레임이 바뀌므로 투구 횟수가 아닌 프레임을 증가, 이는 게임 종료이자 에러의 조건을 충족
                    }
                    else
                    {
                        score[frame][throwBall++] = pin;
                        remain -= pin;
                    }
                    if(remain == 0)
                        remain = 10;
                }
                printScore();
                Console.WriteLine();
                totalScore();
                Console.WriteLine("\n");
            }
            else//1~9프레임
            {
                if(throwBall == 0 && pin == 10)//스트라이크 조건
                {
                    score[frame][throwBall] = pin;
                    score[frame++][throwBall + 1] = 0;
                }
                else//나머지
                {
                    if(throwBall == 0)
                    {
                        score[frame][throwBall++] = pin;
                        remain -= pin;
                    }
                    else
                    {
                        score[frame++][throwBall] = pin;
                        remain = 10;
                        throwBall = 0;
                    }
                }
                printScore();
                Console.WriteLine();
                totalScore();
                Console.WriteLine("\n");
            }
        }

        public void printScore()
        {
            for(int i = 0; i < endFrame - 1; i++)//1~9 프레임 출력
            {
                int remainP = 10;
                Console.Write((i+1) + ":[");
                for(int j = 0; j < 2; j++)//투구 순서
                {
                    if (score[i][0] == 10)//스트라이크인 경우
                    {
                        Console.Write(" X ");
                        break;//다음 프레임으로 넘어감
                    }
                    else if(remainP == score[i][j])//스페어인경우
                    {
                        Console.Write("/");
                    }
                    else
                    {
                        if(score[i][j] == -1)//투구안한경우
                        {
                            Console.Write(" ");
                        }
                        else
                        {
                            Console.Write(score[i][j]);
                            remainP -= score[i][j];
                        }                        
                    }
                    if(j != 1)
                        Console.Write(",");
                }
                Console.Write("]   ");
            }            

            Console.Write("10:[");
            int remainLast = 10;
            for (int i = 0; i < 3; i++)//10 프레임 출력
            {                
                if(score[endFrame - 1][i] == 10 && remainLast - score[endFrame-1][i] == 0)//스트라이크
                {
                    Console.Write("X");
                }
                else if(remainLast == score[endFrame - 1][i])
                {
                    Console.Write("/");
                    remainLast = 10;
                }
                else
                {
                    if(score[endFrame-1][i] == -1)
                    {
                        Console.Write(" ");
                    }
                    else
                    {
                        Console.Write(score[endFrame - 1][i]);
                        remainLast -= score[endFrame - 1][i];
                        if (remainLast == 0)
                            remainLast = 10;
                    }
                }
                if(i != 2)
                    Console.Write(",");
            }
            Console.Write("]   ");
        }

        public void totalScore()
        {
            for(int i = 0; i < endFrame - 1; i++) //1~9프레임까지 
            {
                Console.Write((i + 1) + ":[");
                if (score[i][1] != -1)//두번째 투구까지 끝난 경우
                {
                    if (score[i][0] == 10)//스트라이크
                    {
                        if (score[i + 1][0] != -1 && score[i + 1][1] != -1) //다음 프레임 고려
                        {
                            if(i == 0)
                            {
                                if(score[i+1][0] == 10)//스트라이크
                                {
                                    if (score[i + 2][0] != -1)//총점 계산
                                    {
                                        total[i] = 10 + score[i + 1][0] + score[i + 2][0];
                                        printInFormat(i);
                                    }
                                    else
                                    {
                                        Console.Write("   ");
                                    }
                                }
                                else
                                {
                                    total[i] = 10 + score[i + 1][0] + score[i + 1][1];
                                    printInFormat(i);
                                }                                
                            }
                            else if(i == 8)
                            {
                                if (score[i + 1][0] == 10)//연속 스트라이크인 경우
                                {
                                    if (score[i + 1][1] != -1)
                                    {
                                        total[i] = total[i - 1] + 10 + score[i + 1][0] + score[i + 1][1];
                                        printInFormat(i);
                                    }
                                    else
                                    {
                                        Console.Write("   ");
                                    }
                                }
                                else
                                {
                                    total[i] = total[i - 1] + 10 + score[i + 1][0] + score[i + 1][1];
                                    printInFormat(i);
                                }
                            }
                            else
                            {
                                if(score[i + 1][0] == 10)//연속 스트라이크인 경우
                                {
                                    if(score[i+2][0] != -1)
                                    {
                                        total[i] = total[i - 1] + 10 + score[i + 1][0] + score[i + 2][0];
                                        printInFormat(i);
                                    }
                                    else
                                    {
                                        Console.Write("   ");
                                    }
                                }
                                else
                                {
                                    total[i] = total[i - 1] + 10 + score[i + 1][0] + score[i + 1][1];
                                    printInFormat(i);
                                }
                            }                            
                        }
                        else
                        {
                            Console.Write("   ");
                        }
                    }
                    else if (score[i][0] + score[i][1] == 10)//스페어
                    {
                        if (score[i + 1][0] != -1)
                        {
                            if(i == 0)
                            {
                                total[i] = score[i][0] + score[i][1] + score[i + 1][0];
                                printInFormat(i);
                            }
                            else
                            {
                                total[i] = total[i-1] + score[i][0] + score[i][1] + score[i + 1][0];
                                printInFormat(i);
                            }
                            
                        }
                        else
                        {
                            Console.Write("   ");
                        }
                    }
                    else
                    {
                        if(i == 0)
                        {
                            total[i] = score[i][0] + score[i][1];
                            printInFormat(i);
                        }
                        else if(score[i][1] != -1)
                        {
                            total[i] = total[i-1] + score[i][0] + score[i][1];
                            printInFormat(i);
                        }
                        else
                        {
                            Console.Write("   ");
                        }
                    }
                }
                else
                {
                    Console.Write("   ");
                }
                Console.Write("]   ");
            }

            Console.Write("10:[  ");
            if(score[endFrame - 1][2] != -1)
            {
                total[endFrame-1] = total[endFrame-2] + score[endFrame-1][0] + score[endFrame-1][1] + score[endFrame-1][2];
                printInFormat(endFrame - 1);
            }
            else if(score[endFrame - 1][1] != 10 && score[endFrame - 1][1] != -1)
            {
                total[endFrame - 1] = total[endFrame - 2] + score[endFrame - 1][0] + score[endFrame - 1][1];
                printInFormat(endFrame - 1);
            }
            else
            {
                Console.Write("   ");
            }
            Console.Write("]");
        }

        public void printInFormat(int i)//출력 칸의 양식(칸의 길이)을 맞추기 위한 출력함수
        {
            if (total[i] < 100 && total[i]>9)
                Console.Write(" " + total[i]);
            else if(total[i] < 10)
                Console.Write("  " + total[i]);
            else
                Console.Write(total[i]);
        }
    }

    internal class Program
    {
        public static void Main(string[] args)
        {
            var game = new Game();

            /*
             * test case 1
             */

            game.KnockedDownPins(4);
            game.KnockedDownPins(6);
            game.KnockedDownPins(5);
            game.KnockedDownPins(5);
            game.KnockedDownPins(10);
            game.KnockedDownPins(6);


            /*
             * test case 2
            
            game.KnockedDownPins(0);
            game.KnockedDownPins(0);

            game.KnockedDownPins(11);
            game.KnockedDownPins(0);
            game.KnockedDownPins(0);

            game.KnockedDownPins(0);
            game.KnockedDownPins(0);

            game.KnockedDownPins(1);
            game.KnockedDownPins(1);

            game.KnockedDownPins(10);

            game.KnockedDownPins(10);

            game.KnockedDownPins(10);

            game.KnockedDownPins(10);

            game.KnockedDownPins(10);

            game.KnockedDownPins(6);
            game.KnockedDownPins(4);
            game.KnockedDownPins(10);
            */


            /*
             * test case 3
            
              game.KnockedDownPins(10);
              game.KnockedDownPins(10);
              game.KnockedDownPins(10);
              game.KnockedDownPins(10);
              game.KnockedDownPins(10);
              game.KnockedDownPins(10);
              game.KnockedDownPins(10);
              game.KnockedDownPins(10);
              game.KnockedDownPins(10);
              game.KnockedDownPins(10);
              game.KnockedDownPins(10);
              game.KnockedDownPins(10);
            */

        }
    }
}
