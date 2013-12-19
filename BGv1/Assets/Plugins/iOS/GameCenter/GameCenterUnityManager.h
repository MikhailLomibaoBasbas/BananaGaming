//
//  GameCenterUnityManager.h
//  Unity-iPhone
//
//  Created by Mikhail Lomibao Basbas on 3/13/13.
//
//

#import <Foundation/Foundation.h>

#define UNITY_GCLISTENER_GAMEOBJECT_NAME                "GameCenterListener"
#define UNITY_GCLISTENER_ONRETRIEVETOPSCORE             "OnGcRetrieveTopScoreResponded"
#define UNITY_GCLISTENER_ONRETRIEVETOPSCORERSNAME       "OnGcRetrieveTopScorersNameResponded"
#define UNITY_GCLISTENER_DIDNOTRETRIEVEHIGHSCORES       "OnGcDidNotRetrieveHighScoresResponded"
#define UNITY_GCLISTENER_ONREPORTACHIEVEMENTS           "OnGcReportAchievementsResponded"

@interface GameCenterUnityManager : NSOperation

+(GameCenterUnityManager *) sharedManager;

-(BOOL) hasGKGameCenterViewController;

-(BOOL) isAuthenticated;

-(void) reportScore:(NSMutableDictionary *) dictionary;

-(void) showGameCenterViewController;

-(void) authenticateLocalPlayer;

-(void) retrieveTopScroesWithCategory:(NSString *) category;

-(void) reportAchievementsWithAchievements:(NSMutableArray *) achievementIdentifiers;

-(void) resetAchievements;

-(bool) isViewControllerWrapperPresent;

@end