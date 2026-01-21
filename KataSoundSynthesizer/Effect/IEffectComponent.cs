#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataSoundSynthesizer.Effect;

interface IEffectComponent
{
    void Apply(float[,] input);
    void Reset();

    IEffectComponent MakeInstanceCopy();
}
