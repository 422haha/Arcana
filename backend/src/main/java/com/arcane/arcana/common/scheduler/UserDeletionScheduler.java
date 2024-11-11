package com.arcane.arcana.common.scheduler;

import com.arcane.arcana.common.entity.User;
import com.arcane.arcana.user.repository.UserRepository;
import com.arcane.arcana.common.service.RedisService;
import lombok.RequiredArgsConstructor;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Component;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDateTime;
import java.util.List;

/**
 * 탈퇴 후 30일이 지난 사용자들을 영구 삭제하는 스케줄러
 */
@Component
@RequiredArgsConstructor
public class UserDeletionScheduler {

    private final UserRepository userRepository;
    private final RedisService redisService;

    /**
     * 매일 자정에 실행되어, 탈퇴 후 30일이 지난 사용자를 삭제합니다.
     */
    @Scheduled(cron = "0 0 0 * * *") // 매일 자정 실행
    @Transactional
    public void deleteExpiredUsers() {
        LocalDateTime thirtyDaysAgo = LocalDateTime.now().minusDays(30);

        // 탈퇴 후 30일이 지난 사용자 조회
        List<User> usersToDelete = userRepository.findAllByIsDeletedTrueAndDeletedAtBefore(
            thirtyDaysAgo);

        for (User user : usersToDelete) {
            userRepository.delete(user);
            redisService.deleteValue("deleted_user:" + user.getId()); // 관련된 Redis 데이터 삭제
        }
    }
}
