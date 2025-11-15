/////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Audiokinetic Wwise generated include file. Do not edit.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef __WWISE_IDS_H__
#define __WWISE_IDS_H__

#include <AK/SoundEngine/Common/AkTypes.h>

namespace AK
{
    namespace EVENTS
    {
        static const AkUniqueID EVT_FIRSTMISS = 921806467U;
        static const AkUniqueID EVT_MUSIC_PROTOTYPE = 2835304931U;
        static const AkUniqueID EVT_STARTUP = 3332591570U;
        static const AkUniqueID EVT_STOPMISS = 3927013167U;
        static const AkUniqueID EVT_TRANSITION = 1651390242U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace STA_GAMESTATE
        {
            static const AkUniqueID GROUP = 530992273U;

            namespace STATE
            {
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID STA_GAMESTATE_GOOD = 776312611U;
                static const AkUniqueID STA_GAMESTATE_MISS = 2780964940U;
            } // namespace STATE
        } // namespace STA_GAMESTATE

        namespace STA_SOUNDWAY
        {
            static const AkUniqueID GROUP = 4107555126U;

            namespace STATE
            {
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID STA_SOUNDWAY_DRUMS = 1006606944U;
                static const AkUniqueID STA_SOUNDWAY_SYNTH = 1564156445U;
            } // namespace STATE
        } // namespace STA_SOUNDWAY

    } // namespace STATES

    namespace GAME_PARAMETERS
    {
        static const AkUniqueID RTPC_TRANSITIONPROGRESS = 2184396047U;
    } // namespace GAME_PARAMETERS

    namespace BANKS
    {
        static const AkUniqueID INIT = 1355168291U;
        static const AkUniqueID BNK_MASTER = 503036617U;
    } // namespace BANKS

    namespace BUSSES
    {
        static const AkUniqueID BUS_MASTER = 3964155266U;
        static const AkUniqueID BUS_MUSIC = 1162281553U;
    } // namespace BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__
