//
//  SimpleDataBase.h
//  Unity-iPhone
//
//  Created by liyong on 15/10/7.
//
//

#ifndef Unity_iPhone_SimpleDataBase_h
#define Unity_iPhone_SimpleDataBase_h
#import <Foundation/Foundation.h>
@interface SimpleDataBase : NSObject {
    NSMutableDictionary *_dic;
}
@property (retain) NSMutableDictionary * dic;

+ (SimpleDataBase *) sharedInstance;

@end
#endif
