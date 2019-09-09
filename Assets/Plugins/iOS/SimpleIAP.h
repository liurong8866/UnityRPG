//
//  SimpleIAP.h
//  Unity-iPhone
//
//  Created by liyong on 15/10/7.
//
//

#ifndef Unity_iPhone_SimpleIAP_h
#define Unity_iPhone_SimpleIAP_h

#import <Foundation/Foundation.h>
#import "StoreKit/StoreKit.h"


@interface IAPHelper : NSObject <SKProductsRequestDelegate, SKPaymentTransactionObserver> {
    NSArray * _productIds;
    NSArray * _products;
    SKProductsRequest * _request;
}

@property (retain) NSArray * productIds;
@property (retain) NSArray * products;
@property (retain) SKProductsRequest *request;

- (void)requestProducts:(NSArray *)productIdentifiers;
- (id)init;
- (void)buyProductIdentifier:(NSString *)productIdentifier;
- (BOOL)verifyOnServer:(SKPaymentTransaction *)transaction;
- (NSString*)base64Encode:(NSData*)data;

+ (IAPHelper *) sharedHelper;
@end
#endif
