package com.arcane.arcana.common.config;

import jakarta.persistence.EntityManagerFactory;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.orm.jpa.JpaTransactionManager;
import org.springframework.transaction.annotation.EnableTransactionManagement;

/**
 * 트랜잭션 관리 설정을 구성
 */
@Configuration
@EnableTransactionManagement
public class TransactionConfig {

    /**
     * JPA 트랜잭션 매니저 빈 생성
     */
    @Bean
    public JpaTransactionManager transactionManager(EntityManagerFactory entityManagerFactory) {
        return new JpaTransactionManager(entityManagerFactory);
    }
}
