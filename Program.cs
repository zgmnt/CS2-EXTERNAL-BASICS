using System.Numerics;
using Swed64;

Swed swed = new Swed("cs2");
IntPtr client = swed.GetModuleBase("client.dll");

int dwEntityList = 0x19A3328;
int m_hPlayerPawn = 0x7DC; // from CCS PlayerController
int m_iHealth = 0x324; // from CCS PlayerController

int m_iszPlayerName = 0x630; // from C BasePlayerController
int m_vecLastClipCameraPos = 0x12D4;
int m_iTeamNum = 0x3C3;
string teamSTR = "";
string MYteamSTR = "";
double diff = 0;


float posX = 0;
float posY = 0;
float posZ = 0;

float myPosX = 0;
float myPosY = 0;

while (true)
{
    IntPtr t_entityList = swed.ReadPointer(client, dwEntityList);
    IntPtr entityList = swed.ReadPointer(t_entityList, 0x10);

    for (int i = 0; i < 64; i++)
    {
        if (entityList == IntPtr.Zero)
            continue;

        // zyskujemy poczatek pamieci iteracji graczy
        IntPtr currentControler = swed.ReadPointer(entityList, i * 0x78);

        if (currentControler == IntPtr.Zero)
            continue;

        int pawnHandle = swed.ReadInt(currentControler, m_hPlayerPawn);

        if (pawnHandle == 0)
            continue;

        // second
        IntPtr listEntry2 = swed.ReadPointer(t_entityList, 0x8 * ((pawnHandle & 0x7FFF) >> 9) + 0x10);
        IntPtr currentPawn = swed.ReadPointer(listEntry2, 0x78 * (pawnHandle & 0x1FF));

        // values
        string name = swed.ReadString(currentControler, m_iszPlayerName, 16);
        uint health = swed.ReadUInt(currentPawn, m_iHealth);

        // testy
        posX = swed.ReadVec(currentPawn, m_vecLastClipCameraPos)[0];
        posY = swed.ReadVec(currentPawn, m_vecLastClipCameraPos)[1];
        posZ = swed.ReadVec(currentPawn, m_vecLastClipCameraPos)[2];
        int team = swed.ReadInt(currentPawn, m_iTeamNum);

        if (team == 2)
        {
            teamSTR = "TT";
        }
        else
        {
            teamSTR = "CT";
        }

        if (i == 1)
        {
            myPosX = posX;
            myPosY = posY;
            MYteamSTR = teamSTR;
        }

        diff = Math.Sqrt((myPosX - posX) * (myPosX - posX) + (myPosY - posY) * (myPosY - posY)) / 50;

        if (team == 3)
        {
            if (health > 0)

            {
                if (diff < 8)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }

                Console.WriteLine($"{teamSTR}  {name}  {Math.Round(diff, 0)} m");
            }

        }
    }

    System.Threading.Thread.Sleep(1000);
    Console.Clear();
}