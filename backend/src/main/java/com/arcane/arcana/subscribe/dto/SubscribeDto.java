package com.arcane.arcana.subscribe.dto;

import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 * 구독 요청을 위한 DTO
 */
@Getter
@Setter
@NoArgsConstructor
public class SubscribeDto {

    private String subscribe; // 구독자의 이메일
}
